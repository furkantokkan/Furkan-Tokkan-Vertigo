using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Game.Utilities;
using UniRx;
using DG.Tweening;

namespace Game.UI
{
    public class DoubleButtonPopupView : PopupView
    {
        [SerializeField] private Button selection1;
        [SerializeField] private Button selection2;
        
        public Action onSelection1;
        public Action onSelection2;

        private void Awake()
        {
            selection1.onClick.AddListener(() =>
            {
                onSelection1?.Invoke();
                Hide();
            });
            selection2.onClick.AddListener(() =>
            {
                onSelection2?.Invoke();
                Hide();
            });
        }
        public override void Show()
        {
            base.Show();
            transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        protected override void OnPopupClosed()
        {
            base.OnPopupClosed();
        }

        private void OnDestroy()
        {
            selection1.onClick.RemoveAllListeners();
            selection2.onClick.RemoveAllListeners();
        }
    }
}
