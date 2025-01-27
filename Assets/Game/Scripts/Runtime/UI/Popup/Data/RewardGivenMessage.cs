using Game.Collectable;
using UnityEngine;

namespace Game.UI.Popup
{
    public class RewardGivenMessage : PopupMessage
    {
        public WheelSlot Slot { get; private set; }
        public int CurrentWave { get; private set; }

        public RewardGivenMessage(WheelSlot slot, int currentWave)
            : base($"Reward: {slot.item.itemName}", $"You got {slot.item.itemName} in Wave {currentWave}")
        {
            Slot = slot;
            CurrentWave = currentWave;
        }
    }
}