using Sirenix.OdinInspector;
using System.Linq;
using Game.Collectable;
using UnityEditor;
using UnityEngine;
using Game.Boxes;
using Sirenix.Utilities.Editor;

[System.Serializable]
public class BoxContentItems
{
    [HorizontalGroup("Row1", MarginRight = 5)]
    [BoxGroup("Row1/Slot 1")]
    [HideLabel]
    public WheelSlot slot1 = new WheelSlot();

    [HorizontalGroup("Row1", MarginRight = 5)]
    [BoxGroup("Row1/Slot 2")]
    [HideLabel]
    public WheelSlot slot2 = new WheelSlot();

    [HorizontalGroup("Row1")]
    [BoxGroup("Row1/Slot 3")]
    [HideLabel]
    public WheelSlot slot3 = new WheelSlot();

    [PropertySpace(0, 2)]
    [HorizontalGroup("Row2", MarginRight = 5)]
    [BoxGroup("Row2/Slot 4")]
    [HideLabel]
    public WheelSlot slot4 = new WheelSlot();

    [HorizontalGroup("Row2", MarginRight = 5)]
    [BoxGroup("Row2/Slot 5")]
    [HideLabel]
    public WheelSlot slot5 = new WheelSlot();

    [HorizontalGroup("Row2")]
    [BoxGroup("Row2/Slot 6")]
    [HideLabel]
    public WheelSlot slot6 = new WheelSlot();

    public WheelSlot[] Slots => new[] { slot1, slot2, slot3, slot4, slot5, slot6 };

    private Box box;

    public BoxContentItems(Box box)
    {
        this.box = box;
    }
}