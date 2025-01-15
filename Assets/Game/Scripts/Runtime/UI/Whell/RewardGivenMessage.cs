using Game.Collectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Wheel 
{
    public class RewardGivenMessage
    {
        public WheelItem Reward { get; private set; }
        public int CurrentWave { get; private set; }

        public RewardGivenMessage(WheelItem reward, int currentWave)
        {
            Reward = reward;
            CurrentWave = currentWave;
        }
    }
}
