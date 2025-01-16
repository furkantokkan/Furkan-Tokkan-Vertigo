using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class RewardItemContent : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI amountText;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnValidate()
        {
            if (itemIcon == null)
            {
                itemIcon = GetComponentInChildren<Image>();
            }
            if (amountText == null)
            {
                amountText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        public void Setup(Sprite icon, int amount)
        {
            itemIcon.sprite = icon;
            UpdateAmount(amount);
        }

        public void UpdateAmount(int amount)
        {
            amountText.text = amount.ToString();
            PlayPunchAnimation();
        }

        private void PlayPunchAnimation()
        {
            Vector3 originalScale = rectTransform.localScale;
            DOTween.Sequence()
                .Append(rectTransform.DOScale(originalScale * 1.2f, 0.1f))
                .Append(rectTransform.DOScale(originalScale, 0.1f))
                .SetEase(Ease.OutBack);
        }

        public bool MatchesIcon(Sprite icon)
        {
            return itemIcon.sprite == icon;
        }

        public void Clear()
        {
            itemIcon.sprite = null;
            amountText.text = string.Empty;
        }
    }
}
