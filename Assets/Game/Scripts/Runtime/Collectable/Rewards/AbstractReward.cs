using System.Collections;
using System.Collections.Generic;
using Game.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Collectable
{
    public abstract class AbstractReward : AbstractCollectable
    {
        [VerticalGroup("Split/Right"), LabelWidth(120)]
        public RewardType RewardType;
    }
}
