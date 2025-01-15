using Game.UI.Progressbar;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine;
using Game.Utilities;
using TMPro;
using UniRx;

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
        private ProgressBarWaveManager waveManager;
        private ReactiveProperty<int> currentWave;

        private void Awake()
        {
            InitializeDependencies();
            InitializeReactiveProperties();
        }

        private void InitializeDependencies()
        {
            poolManager = new ProgressBarPoolManager(waveTextPrefab);
            waveManager = new ProgressBarWaveManager(poolManager, settings, waveContainer);
            progressBar = new DOTweenProgressBar(progressImage, settings);
        }

        private void InitializeReactiveProperties()
        {
            currentWave = new ReactiveProperty<int>(1);
            currentWave.Subscribe(OnWaveChanged).AddTo(this);
        }

        private void Start()
        {
            
        }

        private void OnWaveChanged(int newWave)
        {
            progressBar.PlayTransitionAnimation(newWave);
        }

        [FoldoutGroup("Play Mode Test")]
        [Button("Advance Wave")]
        private void AdvanceWave()
        {
            currentWave.Value++;
        }

        [FoldoutGroup("Play Mode Test")]
        [Button("Play Transition")]
        private void PlayTransition()
        {
            progressBar.PlayTransitionAnimation(currentWave.Value);
        }
        private void OnDestroy()
        {
            progressBar?.Stop();
            poolManager?.Clear();
            currentWave?.Dispose();
        }
    }
}