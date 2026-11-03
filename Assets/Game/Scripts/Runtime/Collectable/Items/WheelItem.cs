using Game.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Collectable
{
    public class WheelItem : AbstractCollectable
    {
        [VerticalGroup("Split/Right"), LabelWidth(120)]
        [LabelText("Item Type"), ValueDropdown(nameof(ItemTypeIds))]
        public string itemTypeId;
        [TableList(AlwaysExpanded = true, ShowIndexLabels = true)]
        [SerializeField] public List<AbstractReward> rewardsToGive = new List<AbstractReward>();

        public ItemType ItemType
        {
            get
            {
                if (!Enum.TryParse(itemTypeId, out ItemType itemType))
                    Debug.LogError($"Unknown item type id '{itemTypeId}' on {name}", this);

                return itemType;
            }
        }

        private static string[] ItemTypeIds => Enum.GetNames(typeof(ItemType));
    }
}