using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Editor
{
    public interface IMenuState 
    {
        bool CanHandle(OdinMenuItem menu);
    }
}
