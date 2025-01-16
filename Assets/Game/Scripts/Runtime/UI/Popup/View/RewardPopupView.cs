using DG.Tweening;
using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class RewardPopupView : PopupView
    {
        [SerializeField] private Image rewardIcon;
        [SerializeField] private float autoCloseDelay = 2f;
        [SerializeField] private float minTimeBeforeClick = 0.3f;

        private Sequence showSequence;
        private bool isReadyToClose;

        private void OnMouseDown()
        {
            if (isReadyToClose && Input.GetMouseButtonDown(0))
            {
                ClosePopup();
            }
        }
        private void OnDisable()
        {
            MessageBroker.Default.Publish(GameConst.Events.REWARD_POPUP_CLOSED);
            showSequence?.Kill();
            StopAllCoroutines();
            isReadyToClose = false;
        }
        public override void Show()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            isReadyToClose = false;

            showSequence = DOTween.Sequence()
                .Append(transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack))
                .AppendCallback(() => StartCoroutine(EnableCloseAfterDelay()))
                .AppendInterval(autoCloseDelay)
                .Append(transform.DOScale(0f, 0.2f).SetEase(Ease.InBack))
                .OnComplete(() => OnPopupClosed());
        }

        private IEnumerator EnableCloseAfterDelay()
        {
            yield return new WaitForSeconds(minTimeBeforeClick);
            isReadyToClose = true;
        }

        public override void SetContent(string title, string message, Sprite icon = null)
        {
            base.SetContent(title, message, icon);
            if (icon != null)
            {
                rewardIcon.gameObject.SetActive(true);
                rewardIcon.sprite = icon;
            }
            else
            {
                rewardIcon.gameObject.SetActive(false);
            }
        }

        private void ClosePopup()
        {
            showSequence?.Kill();
            transform.DOScale(0f, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => OnPopupClosed());
        }

    }
}
