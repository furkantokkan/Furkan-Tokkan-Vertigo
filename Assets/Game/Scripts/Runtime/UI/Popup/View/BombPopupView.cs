using DG.Tweening;
using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class BombPopupView : PopupView
    {
        [SerializeField] private Button giveUpButton;
        [SerializeField] private Button reviveButton;

        public Action onGiveUp;
        public Action onRevive;

        private void Awake()
        {
            giveUpButton.onClick.AddListener(() =>
            {
                onGiveUp?.Invoke();
                Hide();
            });

            reviveButton.onClick.AddListener(() =>
            {
                onRevive?.Invoke();
                Hide();
            });
        }

        private void OnDisable()
        {
            MessageBroker.Default.Publish(GameConst.Events.BOMB_POPUP_CLOSED);
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
            giveUpButton.onClick.RemoveAllListeners();
            reviveButton.onClick.RemoveAllListeners();
        }
    }

}
