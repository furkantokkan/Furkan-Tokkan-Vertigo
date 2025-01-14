using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Collectable
{
    public class NumberBasedReward : AbstractReward
    {
        [SerializeField] private int baseValue;
        public int BaseValue => baseValue;
    }
}
