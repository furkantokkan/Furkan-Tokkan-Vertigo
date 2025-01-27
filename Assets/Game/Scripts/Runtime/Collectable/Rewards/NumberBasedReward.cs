using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Collectable
{
    public class NumberBasedReward : AbstractReward
    {
        [VerticalGroup("Split/Right"), LabelWidth(120)]
        [SerializeField] private int baseValue;
        public int BaseValue => baseValue;
    }
}
