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
        private WheelSession session;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            session = new WheelSession(settings.Box, MessageBroker.Default);
            fortuneWheel = new FortuneWheel(wheelTransform, settings);
            wheelImage = wheelTransform.GetComponent<Image>();
            wheelIndicatorImage = wheelIndicator.GetComponent<Image>();
            spinButton.onClick.AddListener(SpinTheWheel);
        }

        private void OnEnable()
        {
            fortuneWheel.Initialize();
            ShowCurrentWave();
            SubscribeToEvents();
        }
        private void OnValidate()
        {
            spinButton = GetComponentInChildren<Button>();
        }
        private void OnDisable()
        {
            disposables.Clear();
        }
        private void OnDestroy()
        {
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
            fortuneWheel.UpdateVisuals(session.CurrentWave);

            var visuals = settings.GetVisuals(session.CurrentContent.WaveType);
            wheelImage.sprite = visuals.wheel;
            wheelIndicatorImage.sprite = visuals.indicator;
        }
        private void ResetGame()
        {
            session.Reset();
            fortuneWheel.Clear();
            fortuneWheel.Initialize();
            ShowCurrentWave();
        }

        [Button("Spin Wheel", ButtonSizes.Medium)]
        public void SpinTheWheel()
        {
            if (fortuneWheel.IsSpinning) return;

            var selectedSlot = session.PickSlot();
            if (selectedSlot == null) return;

            fortuneWheel.Spin(selectedSlot, () =>
            {
                session.CompleteSpin(selectedSlot);
                ShowCurrentWave();
            });
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
