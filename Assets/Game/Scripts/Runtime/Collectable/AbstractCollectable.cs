using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Collectable
{
    public abstract class AbstractCollectable : SerializedScriptableObject
    {
        [SerializeField, NameCheck(typeof(AbstractCollectable))] public string ItemName;
        public Sprite ItemSprite;
    }
}
