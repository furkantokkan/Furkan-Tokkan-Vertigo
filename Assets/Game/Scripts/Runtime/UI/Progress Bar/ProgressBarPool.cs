using Game.Utilities;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ProgressBarPool : ObjectPoolManager<TextMeshProUGUI>
    {
        public ProgressBarPool(TextMeshProUGUI prefab) : base(prefab, GameConst.PROGRESS_BAR_POOL_CAPACITY) 
        {
        }

        protected override TextMeshProUGUI CreateItem()
        {
            var item = base.CreateItem();
            SetupTextDefaults(item);
            return item;
        }

        protected override void OnGetItem(TextMeshProUGUI text)
        {
            base.OnGetItem(text);
            ResetTextProperties(text);
        }

        private void SetupTextDefaults(TextMeshProUGUI text)
        {
            text.alignment = TextAlignmentOptions.Center;
            text.enableAutoSizing = false;
            text.overflowMode = TextOverflowModes.Overflow;
        }

        private void ResetTextProperties(TextMeshProUGUI text)
        {
            text.alpha = 1f;
            text.transform.localScale = Vector3.one;

            var rectTransform = text.rectTransform;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}