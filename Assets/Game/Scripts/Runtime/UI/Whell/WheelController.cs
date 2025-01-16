using Game.Collectable;
using Game.UI.Popup;
using Game.Utilities;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.UI.Wheel
{
    public class WheelController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private WheelSettings settings;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Transform wheelIndicator;

        private IFortuneWheel fortuneWheel;
        private int currentWave = 1;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            fortuneWheel = new FortuneWheel(wheelTransform, wheelIndicator, settings);
        }

        private void OnEnable()
        {
            fortuneWheel.Initialize();
            fortuneWheel.UpdateVisuals(currentWave);
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            MessageBroker.Default.Receive<string>()
                .Where(msg => msg == GameConst.Events.REWARD_POPUP_CLOSED ||
                             msg == GameConst.Events.BOMB_POPUP_CLOSED)
                .Subscribe(_ => fortuneWheel.UpdateVisuals(currentWave))
                .AddTo(disposables);
        }

        [Button("Spin Wheel", ButtonSizes.Medium)]
        public void SpinTheWheel()
        {
            if (fortuneWheel.IsSpinning) return;

            var selectedWheelSlot = settings.Box.GetReward(currentWave);
            if (selectedWheelSlot == null || selectedWheelSlot.item == null) return;

            fortuneWheel.Spin(selectedWheelSlot, () =>
            {
                SendReward(selectedWheelSlot);
                currentWave++;
                fortuneWheel.UpdateVisuals(currentWave);
            });
        }

        private void SendReward(WheelSlot wheelSlot)
        {
            if (wheelSlot.item.itemType == ItemType.Bomb)
            {
                MessageBroker.Default.Publish(new BombGivenMessage(wheelSlot.item, currentWave));
            }
            else
            {
                MessageBroker.Default.Publish(new RewardGivenMessage(wheelSlot.item, currentWave));
            }
        }

        private void OnDestroy()
        {
            disposables.Clear();
            fortuneWheel.Clear();
        }

#if UNITY_EDITOR
        [Button("Reset Wave", ButtonSizes.Medium)]
        private void ResetWave()
        {
            currentWave = 1;
            fortuneWheel.UpdateVisuals(currentWave);
            MessageBroker.Default.Publish(GameConst.Events.GAME_OVER);
        }
#endif
    }
}