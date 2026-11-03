using UnityEngine;

[System.Serializable]
public class BoxContentItems
{
    private const int DEFAULT_SLOT_COUNT = 8;

    [SerializeField]
    private WheelSlot[] slots = CreateDefaultSlots();

    public WheelSlot[] Slots => slots;

    private static WheelSlot[] CreateDefaultSlots()
    {
        var defaultSlots = new WheelSlot[DEFAULT_SLOT_COUNT];
        for (int i = 0; i < defaultSlots.Length; i++)
        {
            defaultSlots[i] = new WheelSlot();
        }

        return defaultSlots;
    }
}
