using DG.Tweening;
using Game.Boxes;
using Game.Collectable;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Wheel
{
    public class FortuneWheel : IFortuneWheel
    {
        private readonly Transform wheelTransform;
        private readonly Transform indicatorTransform;
        private readonly WheelSettings settings;
        private readonly Dictionary<WheelItem, float> itemAngles;
        private readonly List<GameObject> slotObjects;
        private BoxContent currentContent;
        private WheelSlot[] currentSlots;
        private bool isSpinning;

        public bool IsSpinning => isSpinning;

        public FortuneWheel(Transform wheelTransform, Transform indicatorTransform, WheelSettings settings)
        {
            this.wheelTransform = wheelTransform;
            this.indicatorTransform = indicatorTransform;
            this.settings = settings;
            this.itemAngles = new Dictionary<WheelItem, float>();
            this.slotObjects = new List<GameObject>();
        }

        public void Initialize()
        {
            CreateSlots();
            currentContent = null;
        }

        public void Spin(WheelSlot wheelSlot, Action onComplete)
        {
            if (isSpinning || !itemAngles.ContainsKey(wheelSlot.item)) return;

            isSpinning = true;
            float targetAngle = itemAngles[wheelSlot.item];
            float currentRotation = wheelTransform.eulerAngles.z;
            float finalRotation = CalculateTargetRotation(targetAngle);

            currentRotation = ((currentRotation % 360) + 360) % 360;
            float shortestRotation = Mathf.DeltaAngle(currentRotation, finalRotation);
            float totalRotation = (360f * settings.SpinRotations) + shortestRotation;

            Sequence spinSequence = DOTween.Sequence();

            // Hýzlanma fazý
            spinSequence.Append(
                wheelTransform.DORotate(
                    new Vector3(0, 0, currentRotation + (totalRotation * 0.4f)),
                    settings.SpinDuration * 0.3f,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.InQuad)
            );

            // Sabit hýzlý dönüþ fazý
            spinSequence.Append(
                wheelTransform.DORotate(
                    new Vector3(0, 0, currentRotation + (totalRotation * 0.9f)),
                    settings.SpinDuration * 0.5f,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear)
            );

            // Yavaþlama ve hedefi bulma fazý
            spinSequence.Append(
                wheelTransform.DORotate(
                    new Vector3(0, 0, currentRotation + totalRotation),
                    settings.SpinDuration * 0.2f,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.OutQuart)
            );

            spinSequence.SetDelay(0.5f);

            spinSequence.OnComplete(() =>
            {
                isSpinning = false;
                onComplete?.Invoke();
            });
        }

        public void UpdateVisuals(int currentWave)
        {
            var content = settings.Box.contents[currentWave];

            if (currentContent != content)
            {
                currentContent = content;
                currentSlots = content.BoxContentItems.Slots;
                UpdateWheelSprite(content);
                UpdateSlotContents();
            }
        }

        public void Clear()
        {
            foreach (var slot in slotObjects)
            {
                if (slot != null)
                    UnityEngine.Object.Destroy(slot);
            }
            slotObjects.Clear();
            itemAngles.Clear();
            currentContent = null;
            currentSlots = null;
        }

        private void CreateSlots()
        {
            Clear();

            for (int i = 0; i < settings.SlotCount; i++)
            {
                float angle = i * settings.AnglePerSlot;
                Vector3 position = CalculateSlotPosition(angle);

                Vector3 directionToCenter = -position.normalized;
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle + 90);

                var slotObject = UnityEngine.Object.Instantiate(settings.SlotPrefab, wheelTransform);
                slotObject.transform.localPosition = position;
                slotObject.transform.localRotation = rotation;

                var rectTransform = slotObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = settings.SlotSize;
                }

                slotObjects.Add(slotObject);
            }
        }

        private Vector3 CalculateSlotPosition(float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = -Mathf.Sin(radian) * settings.SlotDistance;
            float y = Mathf.Cos(radian) * settings.SlotDistance;
            return new Vector3(x, y, 0);
        }

        private float CalculateTargetRotation(float itemAngle)
        {
            float targetAngle = -itemAngle;
            targetAngle += settings.OffsetAngle;
            return ((targetAngle % 360) + 360) % 360;
        }

        private void UpdateWheelSprite(BoxContent content)
        {
            var indicatorSprite = settings.Box.defaultIndicator;

            if (content.isSuperBox)
            {
                indicatorSprite = settings.Box.superIndicator;
            }
            else if (content.isSafeBox)
            {
                indicatorSprite = settings.Box.safeIndicator;
            }

            indicatorTransform.GetComponent<Image>().sprite = indicatorSprite;
            wheelTransform.GetComponent<Image>().sprite = settings.Box.defaultWheel;
        }

        private void UpdateSlotContents()
        {
            itemAngles.Clear();

            for (int i = 0; i < settings.SlotCount && i < currentSlots.Length; i++)
            {
                if (currentSlots[i].item != null && slotObjects[i] != null)
                {
                    float angle = i * settings.AnglePerSlot;
                    itemAngles[currentSlots[i].item] = angle;
                    UpdateSlotVisuals(i, currentSlots[i]);
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
    }
}