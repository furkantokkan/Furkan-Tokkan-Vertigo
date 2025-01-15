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
        }

        private void CreateSlots()
        {
            ClearSlots();

            for (int i = 0; i < WheelSettings.SLOT_COUNT; i++)
            {
                float angle = i * WheelSettings.ANGLE_PER_SLOT;
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
            float x = Mathf.Cos(radian) * settings.SlotDistance;
            float y = Mathf.Sin(radian) * settings.SlotDistance;
            return new Vector3(x, y, 0);
        }

        private void UpdateSlotContents()
        {
            var content = settings.Box.contents[currentWave - 1];
            var slots = content.BoxContentItems.Slots;

            for (int i = 0; i < WheelSettings.SLOT_COUNT && i < slots.Length; i++)
            {
                if (slots[i].item != null && slotObjects[i] != null)
                {
                    var slotImage = slotObjects[i].GetComponentInChildren<Image>();
                    var slotText = slotObjects[i].GetComponentInChildren<TextMeshProUGUI>();

                    if (slotImage != null)
                    {
                        slotImage.sprite = slots[i].item.ItemSprite;
                        slotImage.preserveAspect = true;

                        var rectTransform = slotImage.rectTransform;
                        rectTransform.sizeDelta = settings.ImageSize;
                        rectTransform.anchoredPosition = new Vector2(0, settings.ImageVerticalOffset);
                    }

                    if (slotText != null && slots[i].item.rewardsToGive.Count > 0)
                    {
                        var reward = slots[i].item.rewardsToGive[0];
                        var amount = slots[i].GetValue<float>(reward);
                        slotText.text = $"x{amount:F0}";

                        slotText.transform.rotation = slotObjects[i].transform.rotation;
                        slotText.transform.localScale = Vector3.one * settings.TextScale;

                        var rectTransform = slotText.rectTransform;
                        rectTransform.anchoredPosition = new Vector2(0, settings.TextVerticalOffset);
                        rectTransform.sizeDelta = settings.TextSize;
                    }
                }
            }
        }

        private void UpdateWheelVisuals()
        {
            var content = settings.Box.contents[currentWave - 1];
            var indicatorSprite = settings.Box.defaultIndicator;

            if (content.isSuperBox)
            {
                indicatorSprite = settings.Box.superIndicator;
                MessageBroker.Default.Publish(GameConst.WAVE_SUPER);
            }
            else if (content.isSafeBox)
            {
                indicatorSprite = settings.Box.safeIndicator;
                MessageBroker.Default.Publish(GameConst.WAVE_SAFE);
            }
            else
            {
                MessageBroker.Default.Publish(GameConst.WAVE_NORMAL);
            }

            wheelIndicator.GetComponent<Image>().sprite = indicatorSprite;
            wheelTransform.GetComponent<Image>().sprite = settings.Box.defaultWheel;
            UpdateSlotContents();
        }

        [Button("Spin Wheel", ButtonSizes.Medium)]
        public void SpinTheWheel()
        {
            if (isSpinning) return;

            isSpinning = true;

            var wheelSlot = settings.Box.GetReward(currentWave);
            if (wheelSlot == null || wheelSlot.item == null)
            {
                Debug.LogError("No reward found!");
                isSpinning = false;
                return;
            }

            SpinWheel(wheelSlot);
        }

        private void SpinWheel(WheelSlot wheelSlot)
        {
            float startRotation = wheelTransform.eulerAngles.z;
            float targetRotation = CalculateTargetRotation(wheelSlot.item);
            float totalRotation = (360f * settings.SpinRotations) + targetRotation;

            Sequence spinSequence = DOTween.Sequence();

            spinSequence.Append(wheelTransform.DORotate(
                    new Vector3(0, 0, startRotation - (totalRotation * settings.SpinAccelerationRatio)),
                    settings.SpinDuration * settings.SpinAccelerationDuration,
                    RotateMode.FastBeyond360)
                .SetEase(settings.SpinEase));

            spinSequence.Append(wheelTransform.DORotate(
                    new Vector3(0, 0, startRotation - totalRotation),
                    settings.SpinDuration * (1 - settings.SpinAccelerationDuration),
                    RotateMode.FastBeyond360)
                .SetEase(settings.EndEase));

            Observable.Timer(TimeSpan.FromSeconds(settings.SpinDuration))
                .Subscribe(_ =>
                {
                    ProcessReward(wheelSlot);
                    currentWave++;
                    isSpinning = false;
                    UpdateWheelVisuals();
                })
                .AddTo(disposables);
        }

        private float CalculateTargetRotation(WheelItem reward)
        {
            int slotIndex = FindRewardSlotIndex(reward);
            return (slotIndex * WheelSettings.ANGLE_PER_SLOT) + WheelSettings.OFFSET_ANGLE;
        }

        private int FindRewardSlotIndex(WheelItem reward)
        {
            var content = settings.Box.contents[currentWave - 1];
            var slots = content.BoxContentItems.Slots;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == reward)
                    return i;
            }

            Debug.LogError($"Reward {reward.name} not found in any slot!");
            return 0;
        }

        private void ProcessReward(WheelSlot wheelSlot)
        {
            if (wheelSlot.item.itemType == ItemType.Bomb)
            {
                MessageBroker.Default.Publish(new PopupMessage("Game Over", "You got the bomb!"));
                return;
            }

            MessageBroker.Default.Publish(new RewardGivenMessage(wheelSlot.item, currentWave));
        }

        private void ClearSlots()
        {
            foreach (var slot in slotObjects)
            {
                if (slot != null)
                    Destroy(slot);
            }
            slotObjects.Clear();
        }

#if UNITY_EDITOR
        [Button("Reset Wave", ButtonSizes.Medium)]
        private void ResetWave()
        {
            currentWave = 1;
            UpdateWheelVisuals();
            MessageBroker.Default.Publish(GameConst.WAVE_RESET);
        }
#endif
    }
}