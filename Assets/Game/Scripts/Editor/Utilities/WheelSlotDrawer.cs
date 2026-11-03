using Game.Collectable;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class WheelSlotDrawer : OdinValueDrawer<WheelSlot>
{
    private const float PREVIEW_SIZE = 48f;
    private const float AMOUNT_WIDTH = 56f;
    private const float WEIGHT_LABEL_WIDTH = 48f;

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var slot = this.ValueEntry.SmartValue;
        var item = slot.item;
        int rewardCount = item != null ? item.rewardsToGive.Count : 0;

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float lineStep = lineHeight + EditorGUIUtility.standardVerticalSpacing;
        var rect = EditorGUILayout.GetControlRect(false, Mathf.Max(PREVIEW_SIZE, (2 + rewardCount) * lineStep));

        DrawItemField(rect.AlignLeft(PREVIEW_SIZE).AlignTop(PREVIEW_SIZE), slot);

        var line = rect.AddXMin(PREVIEW_SIZE + 6).AlignTop(lineHeight);
        GUI.Label(line, item != null ? item.itemName : "Empty slot", item != null ? EditorStyles.boldLabel : EditorStyles.miniLabel);

        using (new EditorGUI.DisabledScope(item == null))
        {
            DrawWeightField(line.AddY(lineStep), slot);
        }

        for (int i = 0; i < rewardCount; i++)
        {
            DrawRewardAmount(line.AddY(lineStep * (2 + i)), slot, item.rewardsToGive[i]);
        }
    }

    private void DrawItemField(Rect rect, WheelSlot slot)
    {
        var texture = slot.item != null ? GUIHelper.GetAssetThumbnail(slot.item.itemSprite, typeof(WheelItem), true) : null;

        EditorGUI.BeginChangeCheck();
        var newItem = SirenixEditorFields.UnityPreviewObjectField(rect, slot.item, texture, typeof(WheelItem)) as WheelItem;
        if (EditorGUI.EndChangeCheck())
        {
            slot.item = newItem;
            slot.weight = newItem != null ? 16.67f : 0f;
            slot.ValidateRewards();
            this.ValueEntry.Values.ForceMarkDirty();
        }
    }

    private void DrawWeightField(Rect rect, WheelSlot slot)
    {
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = WEIGHT_LABEL_WIDTH;

        EditorGUI.BeginChangeCheck();
        float newWeight = EditorGUI.Slider(rect, "Weight", slot.weight, 0, 100);
        if (EditorGUI.EndChangeCheck())
        {
            slot.weight = newWeight;
            this.ValueEntry.Values.ForceMarkDirty();
        }

        EditorGUIUtility.labelWidth = labelWidth;
    }

    private void DrawRewardAmount(Rect rect, WheelSlot slot, AbstractReward reward)
    {
        if (reward == null) return;

        var icon = GUIHelper.GetAssetThumbnail(reward.itemSprite, reward.GetType(), true);
        if (icon != null)
            GUI.DrawTexture(rect.AlignLeft(rect.height), icon, ScaleMode.ScaleToFit);

        string tooltip = reward is NumberBasedReward numberReward ? $"Base value: {numberReward.BaseValue}" : string.Empty;
        GUI.Label(rect.AddXMin(rect.height + 4).SubXMax(AMOUNT_WIDTH + 4), new GUIContent(reward.itemName, tooltip));

        EditorGUI.BeginChangeCheck();
        int newAmount = EditorGUI.IntField(rect.AlignRight(AMOUNT_WIDTH), slot.GetAmount(reward));
        if (EditorGUI.EndChangeCheck())
        {
            slot.SetAmount(reward, newAmount);
            this.ValueEntry.Values.ForceMarkDirty();
        }
    }
}
