using DG.Tweening;
using Game.UI.Progressbar;
using Game.Utilities;
using TMPro;
using UnityEngine;

namespace Game.UI.Progressbar
{
    public class ProgressBarWaveController
    {
        private readonly IPoolManager<TextMeshProUGUI> poolManager;
        private readonly ProgressBarSettings settings;
        private readonly RectTransform waveContainer;
        private readonly TextMeshProUGUI textPrefab;
        private readonly float CENTER_BOX_SIZE = 130f;
        private readonly float BACK_WAVE_SCALE = 0.6f;
        private readonly float BACK_WAVE_ALPHA = 0.3f;
        private readonly float CENTER_SCALE_MULTIPLIER = 1.2f;
        private readonly int INITIAL_WAVE_COUNT = 8;
        private readonly int TOTAL_POSITIONS = 15;

        private Sequence currentSequence;
        private bool isMoving;
        private float[] wavePositionsX;

        public ProgressBarWaveController(
            IPoolManager<TextMeshProUGUI> poolManager,
            ProgressBarSettings settings,
            RectTransform waveContainer)
        {
            this.poolManager = poolManager;
            this.settings = settings;
            this.waveContainer = waveContainer;
            this.textPrefab = poolManager.Prefab;

            CalculateWavePositions();
        }

        private void CalculateWavePositions()
        {
            wavePositionsX = new float[TOTAL_POSITIONS];
            float containerWidth = waveContainer.rect.width;
            float visibleWidth = containerWidth * 0.9f;
            float baseSpacing = visibleWidth / 12f;

            wavePositionsX[7] = 0f;

            float rightStartX = baseSpacing * 1.2f;
            for (int i = 1; i <= 6; i++)
            {
                float xPos = rightStartX + (baseSpacing * (i - 1));
                wavePositionsX[7 + i] = xPos;
            }

            float leftStartX = -baseSpacing * 1.2f;
            for (int i = 1; i <= 6; i++)
            {
                float xPos = leftStartX - (baseSpacing * (i - 1));
                wavePositionsX[7 - i] = xPos;
            }

            wavePositionsX[0] = -containerWidth * 1.1f;
            wavePositionsX[14] = containerWidth * 1.1f;
        }

        public void SetupInitialWaves(int currentWave)
        {
            ClearAllWaves();

            try
            {
                SpawnWaveText(currentWave, wavePositionsX[7], true);

                for (int i = 1; i < INITIAL_WAVE_COUNT; i++)
                {
                    SpawnWaveText(currentWave + i, wavePositionsX[7 + i], false);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in SetupInitialWaves: {e.Message}");
            }
        }

        public void UpdateWaveVisuals(int currentWave)
        {
            if (isMoving) return;

            try
            {
                isMoving = true;
                Stop();
                currentSequence = DOTween.Sequence();

                foreach (RectTransform waveText in waveContainer)
                {
                    var tmp = waveText.GetComponent<TextMeshProUGUI>();
                    int currentPosIndex = GetPositionIndex(waveText.anchoredPosition.x);
                    int targetPosIndex = currentPosIndex - 1;

                    if (targetPosIndex < 0 || targetPosIndex >= TOTAL_POSITIONS)
                        continue;

                    currentSequence.Join(waveText.DOAnchorPosX(wavePositionsX[targetPosIndex], settings.MoveDuration));

                    if (targetPosIndex == 7)
                    {
                        currentSequence.Join(tmp.DOFade(1f, settings.MoveDuration));
                        currentSequence.Join(waveText.DOScale(Vector3.one, settings.MoveDuration));
                        currentSequence.Join(waveText.GetComponent<RectTransform>()
                            .DOSizeDelta(new Vector2(CENTER_BOX_SIZE, CENTER_BOX_SIZE), settings.MoveDuration));
                    }
                    else if (targetPosIndex < 7)
                    {
                        currentSequence.Join(tmp.DOFade(BACK_WAVE_ALPHA, settings.MoveDuration));
                        currentSequence.Join(waveText.DOScale(Vector3.one * BACK_WAVE_SCALE, settings.MoveDuration));
                    }
                    else if (targetPosIndex <= 1)
                    {
                        currentSequence.Join(tmp.DOFade(0f, settings.MoveDuration));
                    }
                }

                currentSequence.OnComplete(() => {
                    CleanupOldWaves();
                    SpawnNewWave(currentWave);
                    isMoving = false;
                });
            }
            catch (System.Exception e)
            {
                isMoving = false;
                Debug.LogError($"Error in UpdateWaveVisuals: {e.Message}");
            }
        }

        private int GetPositionIndex(float position)
        {
            float minDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < wavePositionsX.Length; i++)
            {
                float distance = Mathf.Abs(position - wavePositionsX[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        private TextMeshProUGUI SpawnWaveText(int waveNumber, float positionX, bool isCenter)
        {
            var waveText = poolManager.Get();
            waveText.transform.SetParent(waveContainer, false);

            waveText.text = waveNumber.ToString();
            waveText.fontSize = isCenter ? textPrefab.fontSize * CENTER_SCALE_MULTIPLIER : textPrefab.fontSize;
            waveText.alpha = isCenter ? 1f : (positionX > 0 ? 1f : BACK_WAVE_ALPHA);

            var rectTransform = waveText.rectTransform;
            rectTransform.anchoredPosition = new Vector2(positionX, 0);
            rectTransform.sizeDelta = new Vector2(
                isCenter ? CENTER_BOX_SIZE : textPrefab.rectTransform.sizeDelta.x,
                isCenter ? CENTER_BOX_SIZE : textPrefab.rectTransform.sizeDelta.y);

            rectTransform.localScale = isCenter ? Vector3.one :
                (positionX > 0 ? Vector3.one : Vector3.one * BACK_WAVE_SCALE);

            return waveText;
        }

        private void SpawnNewWave(int currentWave)
        {
            try
            {
                int lastWaveNumber = 0;
                foreach (Transform child in waveContainer)
                {
                    int number = int.Parse(child.GetComponent<TextMeshProUGUI>().text);
                    lastWaveNumber = Mathf.Max(lastWaveNumber, number);
                }

                int newWaveNumber = lastWaveNumber + 1;
                SpawnWaveText(newWaveNumber, wavePositionsX[14], false);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in SpawnNewWave: {e.Message}");
            }
        }

        private void CleanupOldWaves()
        {
            try
            {
                for (int i = waveContainer.childCount - 1; i >= 0; i--)
                {
                    var wave = waveContainer.GetChild(i);
                    var positionX = wave.GetComponent<RectTransform>().anchoredPosition.x;

                    if (positionX <= wavePositionsX[0])
                    {
                        poolManager.Release(wave.GetComponent<TextMeshProUGUI>());
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in CleanupOldWaves: {e.Message}");
            }
        }

        private void ClearAllWaves()
        {
            try
            {
                for (int i = waveContainer.childCount - 1; i >= 0; i--)
                {
                    poolManager.Release(waveContainer.GetChild(i).GetComponent<TextMeshProUGUI>());
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in ClearAllWaves: {e.Message}");
            }
        }

        public void Stop()
        {
            if (currentSequence != null && currentSequence.IsPlaying())
            {
                currentSequence.Kill();
                currentSequence = null;
                isMoving = false;
            }
        }
    }
}