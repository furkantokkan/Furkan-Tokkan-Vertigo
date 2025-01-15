using DG.Tweening;
using Game.UI.Progressbar;
using Game.Utilities;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ProgressBarWaveManager
    {
        private readonly IPoolManager<TextMeshProUGUI> poolManager;
        private readonly ProgressBarSettings settings;
        private readonly RectTransform waveContainer;
        private Sequence currentSequence;

        public ProgressBarWaveManager(
            IPoolManager<TextMeshProUGUI> poolManager,
            ProgressBarSettings settings,
            RectTransform waveContainer)
        {
            this.poolManager = poolManager;
            this.settings = settings;
            this.waveContainer = waveContainer;
        }

        public void Stop()
        {
            if (currentSequence != null && currentSequence.IsPlaying())
            {
                currentSequence.Kill();
                currentSequence = null;
            }
        }

        public void SetupInitialWaves(int currentWave)
        {
            SpawnWaveText(currentWave, Vector2.zero);

            for (int i = 0; i < 6; i++)
            {
                SpawnWaveText(
                    currentWave + i + 1,
                    new Vector2(settings.TextSpacing * (i + 1), 0)
                );
            }
        }

        public void UpdateWaveVisuals(int currentWave)
        {
            Stop();

            currentSequence = DOTween.Sequence();

            foreach (RectTransform waveText in waveContainer)
            {
                var newXPosition = waveText.anchoredPosition.x - settings.TextSpacing;

                if (newXPosition <= settings.LeftPoint)
                {
                    var tmp = waveText.GetComponent<TextMeshProUGUI>();
                    currentSequence.Join(tmp.DOFade(settings.FadeAmount, settings.MoveDuration));
                    currentSequence.Join(waveText.DOScale(Vector3.one * settings.ScaleAmount, settings.MoveDuration));
                }

                currentSequence.Join(waveText.DOAnchorPosX(newXPosition, settings.MoveDuration));
            }

            currentSequence.OnComplete(() => {
                CleanupOldWaves();
                SpawnNewWaves(currentWave);
            });
        }

        private TextMeshProUGUI SpawnWaveText(int waveNumber, Vector2 position)
        {
            var waveText = poolManager.Get();
            waveText.transform.SetParent(waveContainer, false);
            waveText.text = waveNumber.ToString();
            waveText.rectTransform.anchoredPosition = position;
            waveText.rectTransform.localScale = Vector3.one;
            waveText.alpha = 1f;
            return waveText;
        }

        private void SpawnNewWaves(int currentWave)
        {
            SpawnWaveText(currentWave, Vector2.zero);

            SpawnWaveText(
                currentWave + 6,
                new Vector2(settings.TextSpacing * 6, 0)
            );
        }

        private void CleanupOldWaves()
        {
            for (int i = waveContainer.childCount - 1; i >= 0; i--)
            {
                var wave = waveContainer.GetChild(i);
                if (wave.GetComponent<RectTransform>().anchoredPosition.x < settings.LeftPoint - settings.TextSpacing)
                {
                    poolManager.Release(wave.GetComponent<TextMeshProUGUI>());
                }
            }
        }
    }
}