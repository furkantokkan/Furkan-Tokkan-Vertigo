using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Game.Collectable
{
    public abstract class AbstractCollectable : SerializedScriptableObject
    {
        [FormerlySerializedAs("ItemSprite")]
        [PreviewField(75), HideLabel]
        [HorizontalGroup("Split", 80)]
        public Sprite itemSprite;

        [FormerlySerializedAs("ItemName")]
        [Title("Details")]
        [VerticalGroup("Split/Right"), LabelWidth(120)]
        [SerializeField, NameCheck(typeof(AbstractCollectable))] public string itemName;

    }
}
