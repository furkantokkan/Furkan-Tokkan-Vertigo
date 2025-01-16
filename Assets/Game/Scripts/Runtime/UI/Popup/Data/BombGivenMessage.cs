using Game.Collectable;
using Game.UI.Popup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class BombGivenMessage : PopupMessage
    {
        public WheelItem Item { get; private set; }
        public int CurrentWave { get; private set; }
        public BombGivenMessage(WheelItem item, int currentWave)
            : base("Game Over!", "You got the bomb!") 
        {
            Item = item;
            CurrentWave = currentWave;
        }
    }
}