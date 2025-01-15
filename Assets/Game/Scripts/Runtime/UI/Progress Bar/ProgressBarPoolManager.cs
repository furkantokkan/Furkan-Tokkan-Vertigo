using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ProgressBarPoolManager : ObjectPoolManager<TextMeshProUGUI>
    {
        public ProgressBarPoolManager(TextMeshProUGUI prefab) : base(prefab) { }

        protected override void OnGetItem(TextMeshProUGUI text)
        {
            base.OnGetItem(text);
            text.alpha = 1f;
            text.transform.localScale = Vector3.one;
        }
    }
}
