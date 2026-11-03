using Game.Collectable;
using Game.UI.Popup;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Wheel
{
    public class WheelManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private WheelSettings settings;
        [SerializeField] private Button spinButton; 
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Transform wheelIndicator;
        
        private Image wheelImage;
        private Image wheelIndicatorImage; 

        private IFortuneWheel fortuneWheel;
        
        private int currentWave = 0;
        
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            fortuneWheel = new FortuneWheel(wheelTransform, settings);
            wheelImage = wheelTransform.GetComponent<Image>();
            wheelIndicatorImage = wheelIndicator.GetComponent<Image>();
            spinButton.onClick.AddListener(SpinTheWheel);
        }

        private void OnEnable()
        {
            fortuneWheel.Initialize();
            fortuneWheel.UpdateVisuals(currentWave);
            SubscribeToEvents();
            CheckAndPublishWaveType();
        }
        private void OnValidate()
        {
            spinButton = GetComponentInChildren<Button>();
        }
        private void OnDestroy()
        {
            disposables.Clear();
            fortuneWheel.Clear();
        }
        private void SubscribeToEvents()
        {
            MessageBroker.Default.Receive<RewardPopupClosedMessage>()
                .Subscribe(_ => ShowCurrentWave())
                .AddTo(disposables);

            MessageBroker.Default.Receive<BombPopupClosedMessage>()
                .Subscribe(_ => ShowCurrentWave())
                .AddTo(disposables);

            MessageBroker.Default.Receive<GameOverMessage>()
                .Subscribe(_ => ResetGame())
                .AddTo(disposables);
        }
        private void ShowCurrentWave()
        {
            fortuneWheel.UpdateVisuals(currentWave);
            CheckAndPublishWaveType();
        }
        private void ResetGame()
        {
            currentWave = 0;
            fortuneWheel.Clear();
            fortuneWheel.Initialize();
            fortuneWheel.UpdateVisuals(currentWave);
            CheckAndPublishWaveType();
        }
        private void CheckAndPublishWaveType()
        {
            var content = settings.Box.contents[currentWave];

            if (content.isSuperBox)
            {
                MessageBroker.Default.Publish(new WaveTypeChangedMessage(WaveType.Super));
                SetIndicatorAndWheel(settings.Box.superWheel, settings.Box.superIndicator);
            }
            else if (content.isSafeBox)
            {
                MessageBroker.Default.Publish(new WaveTypeChangedMessage(WaveType.Safe));
                SetIndicatorAndWheel(settings.Box.safeWheel, settings.Box.safeIndicator);
            }
            else
            {
                MessageBroker.Default.Publish(new WaveTypeChangedMessage(WaveType.Normal));
                SetIndicatorAndWheel(settings.Box.defaultWheel, settings.Box.defaultIndicator);
            }
        }

        private void SetIndicatorAndWheel(Sprite wheel, Sprite indicator)
        {
            wheelImage.sprite = wheel;
            wheelIndicatorImage.sprite = indicator;
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
                CheckAndPublishWaveType();
            });
        }

        private void SendReward(WheelSlot wheelSlot)
        {
            if (wheelSlot.item.ItemType == ItemType.Bomb)
            {
                MessageBroker.Default.Publish(new BombGivenMessage(wheelSlot.item, currentWave));
            }
            else
            {
                MessageBroker.Default.Publish(new RewardGivenMessage(wheelSlot, currentWave));
            }
        }

#if UNITY_EDITOR
        [Button("Reset Wave", ButtonSizes.Medium)]
        private void ResetWave()
        {
            MessageBroker.Default.Publish(new GameOverMessage());
        }
#endif
    }
}