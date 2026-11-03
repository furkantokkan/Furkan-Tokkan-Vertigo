using DG.Tweening;
using Game.Boxes;
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
        private readonly WheelSettings settings;
        private readonly WheelPoolManager poolManager;
        private readonly List<WheelObject> activeSlots;

        private BoxContent currentContent;
        private WheelSlot[] currentSlots;
        private bool isSpinning;

        public bool IsSpinning => isSpinning;

        private float AnglePerSlot => 360f / currentSlots.Length;

        public FortuneWheel(Transform wheelTransform, WheelSettings settings)
        {
            this.wheelTransform = wheelTransform;
            this.settings = settings;
            this.activeSlots = new List<WheelObject>();
            this.poolManager = new WheelPoolManager(settings.SlotPrefab);
        }

        public void Initialize()
        {
            currentContent = null;
        }

        public void Spin(WheelSlot wheelSlot, Action onComplete)
        {
            int slotIndex = currentSlots != null ? Array.IndexOf(currentSlots, wheelSlot) : -1;
            if (isSpinning || slotIndex < 0) return;

            isSpinning = true;
            float targetAngle = slotIndex * AnglePerSlot;
            ExecuteSpinSequence(targetAngle, onComplete);
        }

        public void UpdateVisuals(int currentWave)
        {
            var content = settings.Box.contents[currentWave];
            if (currentContent != content)
            {
                currentContent = content;
                currentSlots = content.BoxContentItems.Slots;

                if (activeSlots.Count != currentSlots.Length)
                    CreateSlots(currentSlots.Length);

                UpdateSlotContents();
            }
        }

        public void Clear()
        {
            isSpinning = false;
            Stop();
            ClearSlots();
            currentContent = null;
            currentSlots = null;
        }

        private void CreateSlots(int slotCount)
        {
            ClearSlots();

            for (int i = 0; i < slotCount; i++)
            {
                var slot = poolManager.Get();
                ConfigureSlotTransform(slot, i);
                activeSlots.Add(slot);
            }
        }

        private void ConfigureSlotTransform(WheelObject slot, int index)
        {
            float angle = index * AnglePerSlot;
            float radian = angle * Mathf.Deg2Rad;
            Vector3 position = new Vector3(
                -Mathf.Sin(radian) * settings.SlotDistance,
                Mathf.Cos(radian) * settings.SlotDistance,
                0
            );

            slot.transform.SetParent(wheelTransform);
            slot.transform.localPosition = position;
            slot.transform.localRotation = Quaternion.Euler(0, 0,
                Mathf.Atan2(-position.normalized.y, -position.normalized.x) * Mathf.Rad2Deg + 90);

            var rectTransform = slot.GetComponent<RectTransform>();
            if (rectTransform != null)
                rectTransform.sizeDelta = settings.SlotSize;
        }

        private void UpdateSlotContents()
        {
            for (int i = 0; i < currentSlots.Length; i++)
            {
                UpdateSlotVisuals(i);
            }
        }
        private void UpdateSlotVisuals(int index)
        {
            var slot = activeSlots[index];
            var item = currentSlots[index].item;
            if (item == null)
            {
                slot.SetContent(null, string.Empty);
                return;
            }

            int amount = item.rewardsToGive.Count > 0
                ? currentSlots[index].GetAmount(item.rewardsToGive[0])
                : 0;

            slot.SetContent(item.itemSprite, $"x{amount}");
        }

        private void ExecuteSpinSequence(float targetAngle, Action onComplete)
        {
            float currentRotation = wheelTransform.eulerAngles.z;
            float finalRotation = (-targetAngle + settings.OffsetAngle + 360) % 360;
            currentRotation = ((currentRotation % 360) + 360) % 360;
            float totalRotation = (360f * settings.SpinRotations) + Mathf.DeltaAngle(currentRotation, finalRotation);

            Sequence spinSequence = DOTween.Sequence();

            spinSequence.Append(CreateSpinTween(currentRotation, totalRotation * 0.3f, settings.SpinDuration * 0.05f, settings.SpinEasePhase1));
            spinSequence.Append(CreateSpinTween(currentRotation, totalRotation * 0.8f, settings.SpinDuration * 0.45f, settings.SpinEasePhase2));
            spinSequence.Append(CreateSpinTween(currentRotation, totalRotation, settings.SpinDuration * 0.5f, settings.SpinEasePhase3));

            spinSequence.OnComplete(() =>
            {
                isSpinning = false;
                onComplete?.Invoke();
            });
        }

        private Tween CreateSpinTween(float currentRotation, float targetRotation, float duration, Ease easeType)
        {
            return wheelTransform.DORotate(
                new Vector3(0, 0, currentRotation + targetRotation),
                duration,
                RotateMode.FastBeyond360
            ).SetEase(easeType);
        }

        private void Stop()
        {
            DOTween.Kill(wheelTransform);
            wheelTransform.rotation = Quaternion.identity;
        }

        private void ClearSlots()
        {
            foreach (var slot in activeSlots)
            {
                if (slot != null)
                    poolManager.Release(slot);
            }
            activeSlots.Clear();
        }
    }
}
