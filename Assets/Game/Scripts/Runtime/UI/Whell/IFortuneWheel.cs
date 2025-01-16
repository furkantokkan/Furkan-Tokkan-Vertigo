using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.UI.Wheel
{
    public interface IFortuneWheel 
    {
        bool IsSpinning { get; }
        void Initialize();
        void Spin(WheelSlot wheelSlot, Action onComplete);
        void UpdateVisuals(int currentWave);
        void Clear();
    }
}
