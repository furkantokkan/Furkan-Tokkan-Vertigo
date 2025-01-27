using Game.Editor;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Collectable
{
    public class WheelItem : AbstractCollectable
    {
        [VerticalGroup("Split/Right"), LabelWidth(120)]
        public ItemType itemType;
        [TableList(AlwaysExpanded = true, ShowIndexLabels = true)]
        [SerializeField] public List<AbstractReward> rewardsToGive = new List<AbstractReward>();
    }
}