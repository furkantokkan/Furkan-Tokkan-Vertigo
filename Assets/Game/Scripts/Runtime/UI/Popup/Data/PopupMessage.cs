using Game.Collectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Popup
{
    public class PopupMessage
    {
        public string Title { get; private set; }
        public string Message { get; private set; }

        public PopupMessage(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
