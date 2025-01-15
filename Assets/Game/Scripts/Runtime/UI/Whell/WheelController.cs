using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Boxes;
using DG.Tweening;
using System;
using UniRx;
using Game.Collectable;
using UnityEngine.UI;
using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Boxes;
using Game.Collectable;
using Game.Utilities;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Wheel
{
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private Box box;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Transform wheelIndicator;
        [SerializeField] private float spinDuration = 3f;
        [SerializeField] private int spinRotations = 5;

        private const float ANGLE_PER_SLOT = 360f / 8;
        private const float OFFSET_ANGLE = -90f;

        private int currentWave = 1;
        private bool isSpinning;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void OnDestroy()
        {
            disposables.Clear();
        }

        private void OnEnable()
        {
            UpdateWheelVisuals();
        }

        private void UpdateWheelVisuals()
        {
            var content = box.contents[currentWave - 1];
            var indicatorSprite = box.defaultIndicator;

            if (content.isSuperBox)
            {
                indicatorSprite = box.superIndicator;
                MessageBroker.Default.Publish(GameConst.WAVE_SUPER);
            }
            else if (content.isSafeBox)
            {
                indicatorSprite = box.safeIndicator;
                MessageBroker.Default.Publish(GameConst.WAVE_SAFE);
            }
            else
            {
                MessageBroker.Default.Publish(GameConst.WAVE_NORMAL);
            }

            wheelIndicator.GetComponent<Image>().sprite = indicatorSprite;
            wheelTransform.GetComponent<Image>().sprite = box.defaultWheel;
        }

        [Button("Spin Wheel", ButtonSizes.Medium)]
        public void SpinTheWheel()
        {
            if (isSpinning) return;

            isSpinning = true;

            var reward = box.GetReward(currentWave);
            if (reward == null)
            {
                Debug.LogError("No reward found!");
                isSpinning = false;
                return;
            }

            SpinWheel(reward);
        }

        private void SpinWheel(WheelItem reward)
        {
            float startRotation = wheelTransform.eulerAngles.z;
            float targetRotation = CalculateTargetRotation(reward);
            float totalRotation = (360f * spinRotations) + targetRotation;

            wheelTransform.DORotate(
                    new Vector3(0, 0, startRotation - totalRotation),
                    spinDuration,
                    RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuart);

            Observable.Timer(TimeSpan.FromSeconds(spinDuration))
                .Subscribe(_ =>
                {
                    GiveReward(reward);
                    currentWave++;
                    isSpinning = false;
                    UpdateWheelVisuals();
                })
                .AddTo(disposables);
        }

        private float CalculateTargetRotation(WheelItem reward)
        {
            int slotIndex = FindRewardSlotIndex(reward);
            float angle = (slotIndex * ANGLE_PER_SLOT) + OFFSET_ANGLE;
            return angle;
        }

        private int FindRewardSlotIndex(WheelItem reward)
        {
            var content = box.contents[currentWave - 1];
            var slots = content.BoxContentItems.Slots;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == reward)
                {
                    return i;
                }
            }

            Debug.LogError($"Reward {reward.name} not found in any slot!");
            return 0;
        }

        private void GiveReward(WheelItem reward)
        {
            Debug.Log($"Giving reward: {reward.name}");
            MessageBroker.Default.Publish(new RewardGivenMessage(reward, currentWave));
        }

#if UNITY_EDITOR
        [Button("Next Wave", ButtonSizes.Medium)]
        private void NextWave()
        {
            currentWave++;
            UpdateWheelVisuals();
        }

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