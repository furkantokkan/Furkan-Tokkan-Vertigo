using Game.Collectable;
using UnityEngine;

namespace Game.UI.Popup
{
    public class RewardGivenMessage : PopupMessage
    {
        public WheelItem Item { get; private set; }
        public int CurrentWave { get; private set; }

        public RewardGivenMessage(WheelItem item, int currentWave)
            : base($"Reward: {item.name}", $"You got {item.name} in Wave {currentWave}")
        {
            Item = item;
            CurrentWave = currentWave;
        }
    }
}