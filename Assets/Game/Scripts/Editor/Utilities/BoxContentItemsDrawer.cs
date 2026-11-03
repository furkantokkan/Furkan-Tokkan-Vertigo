using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class BoxContentItemsDrawer : OdinValueDrawer<BoxContentItems>
{
    private const float MIN_CARD_WIDTH = 240f;
    private const float CARD_SPACING = 4f;

    private float availableWidth = MIN_CARD_WIDTH * 3;

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var slotsProperty = this.Property.Children["slots"];
        if (slotsProperty == null)
        {
            CallNextDrawer(label);
            return;
        }

        var slots = (WheelSlot[])slotsProperty.ValueEntry.WeakSmartValue ?? Array.Empty<WheelSlot>();

        DrawToolbar(slotsProperty, slots);

        // Columns come from the width measured on the previous repaint, so Layout and Repaint agree.
        int columns = Mathf.Max(1, Mathf.FloorToInt((availableWidth + CARD_SPACING) / (MIN_CARD_WIDTH + CARD_SPACING)));
        var widthRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true));
        if (Event.current.type == EventType.Repaint && !Mathf.Approximately(widthRect.width, availableWidth))
        {
            availableWidth = widthRect.width;
            GUIHelper.RequestRepaint();
        }

        float cardWidth = Mathf.Floor((availableWidth - CARD_SPACING * (columns - 1)) / columns) - 2;
        int slotCount = slotsProperty.Children.Count;

        for (int row = 0; row < slotCount; row += columns)
        {
            EditorGUILayout.BeginHorizontal();
            for (int index = row; index < row + columns && index < slotCount; index++)
            {
                if (index > row)
                    GUILayout.Space(CARD_SPACING);

                DrawSlotCard(slotsProperty, index, cardWidth);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawToolbar(InspectorProperty slotsProperty, WheelSlot[] slots)
    {
        float totalWeight = 0f;
        foreach (var slot in slots)
        {
            if (slot != null && slot.item != null)
                totalWeight += slot.weight;
        }

        SirenixEditorGUI.BeginHorizontalToolbar(22);
        GUILayout.Label($"Slots: {slots.Length}    Total weight: {totalWeight:0.##}");
        GUILayout.FlexibleSpace();
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            this.Property.Tree.DelayAction(() => AddSlot(slotsProperty));
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    private void DrawSlotCard(InspectorProperty slotsProperty, int index, float width)
    {
        SirenixEditorGUI.BeginBox(GUILayout.Width(width));
        SirenixEditorGUI.BeginBoxHeader();
        GUILayout.Label($"Slot {index + 1}");
        GUILayout.FlexibleSpace();
        if (SirenixEditorGUI.IconButton(EditorIcons.X, 14, 14, "Remove slot"))
            this.Property.Tree.DelayAction(() => RemoveSlot(slotsProperty, index));
        SirenixEditorGUI.EndBoxHeader();

        slotsProperty.Children[index].Draw(null);
        SirenixEditorGUI.EndBox();
    }

    private static void AddSlot(InspectorProperty slotsProperty)
    {
        var slots = (WheelSlot[])slotsProperty.ValueEntry.WeakSmartValue;
        var newSlots = new WheelSlot[slots.Length + 1];
        Array.Copy(slots, newSlots, slots.Length);
        newSlots[slots.Length] = new WheelSlot();
        slotsProperty.ValueEntry.WeakSmartValue = newSlots;
    }

    private static void RemoveSlot(InspectorProperty slotsProperty, int index)
    {
        var slots = (WheelSlot[])slotsProperty.ValueEntry.WeakSmartValue;
        if (index < 0 || index >= slots.Length) return;

        var newSlots = new WheelSlot[slots.Length - 1];
        Array.Copy(slots, 0, newSlots, 0, index);
        Array.Copy(slots, index + 1, newSlots, index, slots.Length - index - 1);
        slotsProperty.ValueEntry.WeakSmartValue = newSlots;
    }
}
