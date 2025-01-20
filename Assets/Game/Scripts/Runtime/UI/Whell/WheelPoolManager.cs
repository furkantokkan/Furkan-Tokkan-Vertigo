using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utilities;

namespace Game.UI.Wheel
{
    public class WheelPoolManager : ObjectPoolManager<WheelObject>
    {
        public WheelPoolManager(WheelObject prefab, int defaultCapacity = 10) : base(prefab, defaultCapacity)
        {

        }
    }
}
