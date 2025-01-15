using Game.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Collectable
{
    public class WheelItem : AbstractCollectable
    {
        public ItemType itemType;
        public List<AbstractReward> rewardsToGive = new List<AbstractReward>();
    }
}