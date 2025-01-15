using Game.UI.Progressbar;
using Game.UI;
using Game.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine.UI;
using UnityEngine;

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
            .Where(msg => msg == GameConst.PROGRESS_FIRST_STOP)
            .Subscribe(_ => OnFirstStopReached())
            .AddTo(disposables);
    }

    private void Start()
    {
        ResetProgressBar();
        waveController.SetupInitialWaves(currentWave);
    }

    private void ResetProgressBar()
    {
        progressImage.uvRect = new Rect(settings.LeftPoint, 0, 1, 1);
    }

    [FoldoutGroup("Play Mode Test")]
    [Button("Advance Wave")]
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

    [FoldoutGroup("Play Mode Test")]
    [Button("Reset")]
    private void Reset()
    {
        currentWave = 1;
        progressBar.Stop();
        ResetProgressBar();
        waveController.SetupInitialWaves(currentWave);
    }
}