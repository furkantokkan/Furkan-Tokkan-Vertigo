using Game.UI.Progressbar;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine;
using Game.Utilities;
using TMPro;

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

        private IProgressBar progressBar;
        private IPoolManager<TextMeshProUGUI> poolManager;
        private ProgressBarWaveController waveController;
        private int currentWave = 1;

        private void Awake()
        {
            InitializeDependencies();
        }

        private void InitializeDependencies()
        {
            poolManager = new ProgressBarPool(waveTextPrefab);
            waveController = new ProgressBarWaveController(poolManager, settings, waveContainer);
            progressBar = new DOTweenProgressBar(progressImage, settings);
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
            waveController.UpdateWaveVisuals(currentWave);
        }

        [FoldoutGroup("Play Mode Test")]
        [Button("Play Transition")]
        private void PlayTransition()
        {
            progressBar.PlayTransitionAnimation(currentWave);
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

        private void OnDestroy()
        {
            progressBar?.Stop();
            poolManager?.Clear();
        }
    }
}