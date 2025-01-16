using Game.UI.Progressbar;
using Game.UI;
using Game.UI.Popup;
using Game.UI.Wheel;
using Game.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine.UI;
using UnityEngine;

namespace Game.UI.Progressbar
{
    public class TopProgressBarController : MonoBehaviour
    {
        [FoldoutGroup("References")]
        [Required]
        [SerializeField] private RawImage progressImage;

        [FoldoutGroup("References")]
        [Required]
        [SerializeField] private ProgressBarSettings settings;

        [FoldoutGroup("References")]
        [Required]
        [SerializeField] private TextMeshProUGUI waveTextPrefab;

        [FoldoutGroup("References")]
        [Required]
        [SerializeField] private RectTransform waveContainer;

        [FoldoutGroup("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [FoldoutGroup("Colors")]
        [SerializeField] private Color safeColor = Color.green;
        [FoldoutGroup("Colors")]
        [SerializeField] private Color superColor = Color.yellow;

        private IProgressBar progressBar;
        private IPoolManager<TextMeshProUGUI> poolManager;
        private ProgressBarWaveController waveController;
        private int currentWave = 1;
        private CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            InitializeDependencies();
            SubscribeToEvents();
        }
        private void Start()
        {
            ResetProgressBar();
            waveController.SetupInitialWaves(currentWave);
            progressImage.color = normalColor;
        }

        private void Reset()
        {
            currentWave = 1;
            progressBar.Stop();
            ResetProgressBar();
            waveController.SetupInitialWaves(currentWave);
        }
        private void OnDestroy()
        {
            disposables.Clear();
            progressBar?.Stop();
            poolManager?.Clear();
        }

        private void InitializeDependencies()
        {
            poolManager = new ProgressBarPool(waveTextPrefab);
            waveController = new ProgressBarWaveController(poolManager, settings, waveContainer);
            progressBar = new DOTweenProgressBar(progressImage, settings);
        }


        private void SubscribeToEvents()
        {
            MessageBroker.Default
                .Receive<string>()
                .Where(msg => msg == GameConst.Events.PROGRESS_FIRST_STOP)
                .Subscribe(_ => OnFirstStopReached())
                .AddTo(disposables);

            MessageBroker.Default
                .Receive<string>()
                .Where(msg =>
                    msg == GameConst.Events.WAVE_SUPER ||
                    msg == GameConst.Events.WAVE_SAFE ||
                    msg == GameConst.Events.WAVE_NORMAL ||
                    msg == GameConst.Events.GAME_OVER)
                .Subscribe(OnWaveMessage)
                .AddTo(disposables);

            MessageBroker.Default
                .Receive<RewardGivenMessage>()
                .Subscribe(OnRewardGiven)
                .AddTo(disposables);
        }
        private void OnRewardGiven(RewardGivenMessage message)
        {
            currentWave = message.CurrentWave;
            AdvanceWave();
        }

        private void OnWaveMessage(string message)
        {
            switch (message)
            {
                case GameConst.Events.WAVE_SUPER:
                    progressImage.color = superColor;
                    break;
                case GameConst.Events.WAVE_SAFE:
                    progressImage.color = safeColor;
                    Debug.Log("Safe Wave");
                    break;
                case GameConst.Events.WAVE_NORMAL:
                    progressImage.color = normalColor;
                    break;
                case GameConst.Events.GAME_OVER:
                    Reset();
                    break;
            }
        }

        private void ResetProgressBar()
        {
            progressImage.uvRect = new Rect(settings.LeftPoint, 0, 1, 1);
            progressImage.color = normalColor;
        }

        private void AdvanceWave()
        {
            currentWave++;
            progressBar.PlayTransitionAnimation(currentWave);
        }

        private void OnFirstStopReached()
        {
            float remainingDuration = settings.LeftMoveDuration + settings.RightMoveDuration;
            waveController.UpdateWaveVisuals(currentWave, remainingDuration);
        }


#if UNITY_EDITOR
        [FoldoutGroup("Debug")]
        [Button("Test Color Changes")]
        private void TestColors()
        {
            progressImage.color = Random.value > 0.5f ? safeColor : superColor;
        }

        [FoldoutGroup("Debug")]
        [Button("Reset Color")]
        private void ResetColor()
        {
            progressImage.color = normalColor;
        }

        [FoldoutGroup("Debug")]
        [Button("Advance Wave")]
        private void DebugAdvanceWave()
        {
            AdvanceWave();
        }

        [FoldoutGroup("Debug")]
        [Button("Reset Wave")]
        private void DebugResetWave()
        {
            Reset();
        }
#endif
    }
}