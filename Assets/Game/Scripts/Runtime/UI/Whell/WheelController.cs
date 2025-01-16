using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Boxes;
using Game.Collectable;
using Game.Editor;
using Game.UI.Popup;
using Game.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Wheel
{
    public class WheelController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private WheelSettings settings;

        [Header("Wheel Transform")]
        [SerializeField] private Transform wheelTransform;

        [Header("Indicator")]
        [SerializeField] private Transform wheelIndicator;

        private WheelSlot selectedWheelSlot;
        private Dictionary<WheelItem, float> itemAngles = new Dictionary<WheelItem, float>();
        private int currentWave = 1;
        private bool isSpinning;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly List<GameObject> slotObjects = new List<GameObject>();

        private void OnDestroy()
        {
            disposables.Clear();
            ClearSlots();
        }

        private void OnEnable()
        {
            CreateSlots();
            UpdateWheelVisuals();

            MessageBroker.Default.Receive<string>()
                .Where(msg => msg == GameConst.Events.REWARD_POPUP_CLOSED ||
                              msg == GameConst.Events.BOMB_POPUP_CLOSED)
                .Subscribe(msg =>
                {
                    currentWave++;
                    UpdateWheelVisuals();
                    isSpinning = false;
                })
                .AddTo(disposables);
        }

        private void CreateSlots()
        {
            ClearSlots();
            itemAngles.Clear();

            for (int i = 0; i < settings.SlotCount; i++)
            {
                float angle = i * settings.AnglePerSlot;
                Vector3 position = CalculateSlotPosition(angle);

                Vector3 directionToCenter = -position.normalized;
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle + 90);

                var slotObject = Instantiate(settings.SlotPrefab, wheelTransform);
                slotObject.transform.localPosition = position;
                slotObject.transform.localRotation = rotation;

                var rectTransform = slotObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = settings.SlotSize;
                }

                slotObjects.Add(slotObject);
            }

            UpdateSlotContents();
        }


        private Vector3 CalculateSlotPosition(float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = -Mathf.Sin(radian) * settings.SlotDistance;
            float y = Mathf.Cos(radian) * settings.SlotDistance;
            return new Vector3(x, y, 0);
        }


        [Button("Spin Wheel", ButtonSizes.Medium)]
        public void SpinTheWheel()
        {
            if (isSpinning) return;

            isSpinning = true;

            // currentWave kullanýmýný düzelt
            selectedWheelSlot = settings.Box.GetReward(currentWave);
            if (selectedWheelSlot == null || selectedWheelSlot.item == null)
            {
                Debug.LogError("No reward found!");
                isSpinning = false;
                return;
            }

            Debug.Log($"Selected reward: {selectedWheelSlot.item.name} at wave {currentWave}");
            SpinWheelTween(selectedWheelSlot);
        }

        private void UpdateSlotContents()
        {
            var content = settings.Box.contents[currentWave];
            var slots = content.BoxContentItems.Slots;
            itemAngles.Clear();

            for (int i = 0; i < settings.SlotCount && i < slots.Length; i++)
            {
                if (slots[i].item != null && slotObjects[i] != null)
                {
                    // Açýyý saat yönünde hesapla (üstten baþlayarak)
                    float angle = i * settings.AnglePerSlot;
                    itemAngles[slots[i].item] = angle;
                    UpdateSlotVisuals(i, slots[i]);
                }
            }
        }

        private void UpdateSlotVisuals(int index, WheelSlot slot)
        {
            var slotImage = slotObjects[index].GetComponentInChildren<Image>();
            var slotText = slotObjects[index].GetComponentInChildren<TextMeshProUGUI>();

            if (slotImage != null)
            {
                slotImage.sprite = slot.item.ItemSprite;
                slotImage.preserveAspect = true;

                var rectTransform = slotImage.rectTransform;
                rectTransform.sizeDelta = settings.ImageSize;
                rectTransform.anchoredPosition = new Vector2(0, settings.ImageVerticalOffset);
            }

            if (slotText != null && slot.item.rewardsToGive.Count > 0)
            {
                var reward = slot.item.rewardsToGive[0];
                var amount = slot.GetValue<float>(reward);
                slotText.text = $"x{amount:F0}";

                slotText.transform.rotation = slotObjects[index].transform.rotation;
                slotText.transform.localScale = Vector3.one * settings.TextScale;

                var rectTransform = slotText.rectTransform;
                rectTransform.anchoredPosition = new Vector2(0, settings.TextVerticalOffset);
                rectTransform.sizeDelta = settings.TextSize;
            }
        }

        private void UpdateWheelVisuals()
        {
            var content = settings.Box.contents[currentWave];
            var indicatorSprite = settings.Box.defaultIndicator;

            if (content.isSuperBox)
            {
                indicatorSprite = settings.Box.superIndicator;
                MessageBroker.Default.Publish(GameConst.Events.WAVE_SUPER);
            }
            else if (content.isSafeBox)
            {
                indicatorSprite = settings.Box.safeIndicator;
                MessageBroker.Default.Publish(GameConst.Events.WAVE_SAFE);
            }
            else
            {
                MessageBroker.Default.Publish(GameConst.Events.WAVE_NORMAL);
            }

            wheelIndicator.GetComponent<Image>().sprite = indicatorSprite;
            wheelTransform.GetComponent<Image>().sprite = settings.Box.defaultWheel;
            UpdateSlotContents();
        }

        private void SpinWheelTween(WheelSlot wheelSlot)
        {
            if (!itemAngles.ContainsKey(wheelSlot.item))
            {
                Debug.LogError($"Item {wheelSlot.item.name} not found in wheel angles!");
                return;
            }

            float targetAngle = itemAngles[wheelSlot.item];
            float currentRotation = wheelTransform.eulerAngles.z;
            float finalRotation = CalculateTargetRotation(targetAngle);

            // Mevcut rotasyonu normalize et
            currentRotation = ((currentRotation % 360) + 360) % 360;

            // En kýsa rotasyon yolunu hesapla
            float shortestRotation = Mathf.DeltaAngle(currentRotation, finalRotation);

            // Tam turlar için toplam rotasyon
            float totalRotation = (360f * settings.SpinRotations) + shortestRotation;

            Debug.Log($"Current: {currentRotation}, Target: {finalRotation}, Total: {totalRotation}");

            Sequence spinSequence = DOTween.Sequence();

            spinSequence.Append(
                wheelTransform.DORotate(
                    new Vector3(0, 0, currentRotation + (totalRotation * 0.7f)),
                    settings.SpinDuration * 0.4f,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.InQuad)
            );

            spinSequence.Append(
                wheelTransform.DORotate(
                    new Vector3(0, 0, currentRotation + (totalRotation * 0.9f)),
                    settings.SpinDuration * 0.3f,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear)
            );

            spinSequence.Append(
                wheelTransform.DORotate(
                    new Vector3(0, 0, currentRotation + totalRotation),
                    settings.SpinDuration * 0.3f,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.OutQuad)
            );

            spinSequence.SetDelay(0.5f);

            spinSequence.OnComplete(() =>
            {
                SendTheReward(selectedWheelSlot);
            });
        }

        private void SendTheReward(WheelSlot wheelSlot)
        {
            Debug.Log($"Sending reward: {wheelSlot.item.name}");

            if (wheelSlot.item.itemType == ItemType.Bomb)
            {
                MessageBroker.Default.Publish(new BombGivenMessage(wheelSlot.item, currentWave));
            }
            else
            {
                MessageBroker.Default.Publish(new RewardGivenMessage(wheelSlot.item, currentWave));
            }
        }

        private float CalculateTargetRotation(float itemAngle)
        {
            // Ýndikatör üstte (0 derece) olduðu için, hedef açýyý tersine çevir
            float targetAngle = -itemAngle;

            // Offset ekle
            targetAngle += settings.OffsetAngle;

            // 360 derece içinde normalize et
            targetAngle = ((targetAngle % 360) + 360) % 360;

            Debug.Log($"Item angle: {itemAngle}, Target rotation: {targetAngle}");
            return targetAngle;
        }

        private void ClearSlots()
        {
            foreach (var slot in slotObjects)
            {
                if (slot != null)
                    Destroy(slot);
            }
            slotObjects.Clear();
            itemAngles.Clear();
        }

#if UNITY_EDITOR
        [Button("Reset Wave", ButtonSizes.Medium)]
        private void ResetWave()
        {
            currentWave = 1;
            UpdateWheelVisuals();
            MessageBroker.Default.Publish(GameConst.Events.GAME_OVER);
        }
#endif
    }
}