using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI
{
    public class PopupView : MonoBehaviour, IPopupView
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
        }

        protected virtual void OnPopupClosed()
        {
            Hide();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void SetContent(string title, string message, Sprite icon = null)
        {
            titleText.text = title;
            messageText.text = message;
        }
    }
}
