using Game.Boxes;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class BoxContentItems
{
    [HorizontalGroup("Row1", MarginRight = 5, Width = 0.33f)]
    [BoxGroup("Row1/Slot 1")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot1 = new WheelSlot();

    [HorizontalGroup("Row1", MarginRight = 5, Width = 0.33f)]
    [BoxGroup("Row1/Slot 2")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot2 = new WheelSlot();

    [HorizontalGroup("Row1", Width = 0.33f)]
    [BoxGroup("Row1/Slot 3")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot3 = new WheelSlot();

    [PropertySpace(0, 2)]
    [HorizontalGroup("Row2", MarginRight = 5, Width = 0.33f)]
    [BoxGroup("Row2/Slot 4")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot4 = new WheelSlot();

    [HorizontalGroup("Row2", MarginRight = 5, Width = 0.33f)]
    [BoxGroup("Row2/Slot 5")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot5 = new WheelSlot();

    [HorizontalGroup("Row2", Width = 0.33f)]
    [BoxGroup("Row2/Slot 6")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot6 = new WheelSlot();

    [PropertySpace(0, 2)]
    [HorizontalGroup("Row3", MarginRight = 5, Width = 0.33f)]
    [BoxGroup("Row3/Slot 7")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot7 = new WheelSlot();

    [HorizontalGroup("Row3", Width = 0.33f)]
    [BoxGroup("Row3/Slot 8")]
    [HideLabel]
    [SerializeField]
    public WheelSlot slot8 = new WheelSlot();
    public WheelSlot[] Slots => new[] { slot1, slot2, slot3, slot4, slot5, slot6, slot7, slot8 };

    private Box box;

    public BoxContentItems(Box box)
    {
        this.box = box;
    }
}