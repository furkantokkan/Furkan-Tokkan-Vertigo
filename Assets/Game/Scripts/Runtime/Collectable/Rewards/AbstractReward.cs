using System;
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
        [LabelText("Reward Type"), ValueDropdown(nameof(RewardTypeIds))]
        public string rewardTypeId;

        public RewardType RewardType
        {
            get
            {
                if (!Enum.TryParse(rewardTypeId, out RewardType rewardType))
                    Debug.LogError($"Unknown reward type id '{rewardTypeId}' on {name}", this);

                return rewardType;
            }
        }

        private static string[] RewardTypeIds => Enum.GetNames(typeof(RewardType));
    }
}
