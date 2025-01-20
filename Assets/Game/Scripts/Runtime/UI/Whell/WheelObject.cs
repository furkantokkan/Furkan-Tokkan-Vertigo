using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.Wheel
{
    public class WheelObject : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
        public void SetContent(Sprite sprite, string text)
        {
            image.sprite = sprite;
            this.text.text = text;
        }

        private void OnValidate()
        {
            if (image == null)
            {
                image = GetComponentInChildren<Image>();
            }
            if (text == null)
            {
                text = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}
