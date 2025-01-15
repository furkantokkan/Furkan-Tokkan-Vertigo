using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Boxes;
using DG.Tweening;
using System;
using UniRx;
using Game.Collectable;

namespace Game.UI.Wheel
{
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private Box box;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private float spinDuration = 3f;
        [SerializeField] private int spinRotations = 5;

        private int currentWave = 1;
        private bool isSpinning;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void OnDestroy()
        {
            disposables.Clear();
        }

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
                    MessageBroker.Default.Publish(new WaveChangedMessage(currentWave));
                })
                .AddTo(disposables);
        }

        private float CalculateTargetRotation(WheelItem reward)
        {
            // TODO: Implement actual calculation based on your wheel setup
            // Örnek: 6 slot varsa her slot 60 derece
            // Reward'ýn slot numarasýna göre açýyý hesapla
            return 0f;
        }

        private void GiveReward(WheelItem reward)
        {
            // TODO: Implement reward giving logic
            Debug.Log($"Giving reward: {reward.name}");
        }
    }

    public readonly struct WaveChangedMessage
    {
        public readonly int NewWave;

        public WaveChangedMessage(int newWave)
        {
            NewWave = newWave;
        }
    }
}
