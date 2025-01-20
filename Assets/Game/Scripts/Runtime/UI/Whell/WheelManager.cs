using Game.Collectable;
using Game.UI.Popup;
using Game.Utilities;
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
            MessageBroker.Default.Receive<string>()
                .Where(msg => msg == GameConst.Events.REWARD_POPUP_CLOSED ||
                              msg == GameConst.Events.BOMB_POPUP_CLOSED ||
                              msg == GameConst.Events.GAME_OVER)  
                .Subscribe(msg =>
                {
                    if (msg == GameConst.Events.GAME_OVER)
                    {
                        ResetGame();
                    }
                    else
                    {
                        fortuneWheel.UpdateVisuals(currentWave);
                        CheckAndPublishWaveType();
                    }
                })
                .AddTo(disposables);
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
                MessageBroker.Default.Publish(GameConst.Events.WAVE_SUPER);
                SetIndicatorAndWheel(settings.Box.superWheel, settings.Box.superIndicator);
            }
            else if (content.isSafeBox)
            {
                MessageBroker.Default.Publish(GameConst.Events.WAVE_SAFE);
                SetIndicatorAndWheel(settings.Box.safeWheel, settings.Box.safeIndicator);
            }
            else
            {
                MessageBroker.Default.Publish(GameConst.Events.WAVE_NORMAL);
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
            if (wheelSlot.item.itemType == ItemType.Bomb)
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
            MessageBroker.Default.Publish(GameConst.Events.GAME_OVER);
        }
#endif
    }
}