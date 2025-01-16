using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public interface IPopupView 
    {
        void Show();
        void Hide();
        void SetContent(string title, string message, Sprite icon = null);
    }
}
