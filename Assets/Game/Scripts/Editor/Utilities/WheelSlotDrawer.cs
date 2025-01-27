using Game.Collectable;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class WheelSlotDrawer : OdinValueDrawer<WheelSlot>
{
    private bool showRewards;

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var rect = EditorGUILayout.GetControlRect(label != null, 65);

        if (label != null)
        {
            rect.xMin = EditorGUI.PrefixLabel(rect.AlignCenterY(15), label).xMin;
        }
        else
        {
            rect = EditorGUI.IndentedRect(rect);
        }

        WheelItem item = this.ValueEntry.SmartValue.item;
        Texture texture = null;

        if (item != null)
        {
            texture = GUIHelper.GetAssetThumbnail(item.itemSprite, typeof(WheelItem), true);
            GUI.Label(rect.AddXMin(70).AlignMiddle(16), EditorGUI.showMixedValue ? "-" : item.itemName);
        }

        EditorGUI.BeginChangeCheck();
        var newItem = SirenixEditorFields.UnityPreviewObjectField(rect.AlignLeft(65), item, texture, typeof(WheelItem)) as WheelItem;
        if (EditorGUI.EndChangeCheck())
        {
            var value = this.ValueEntry.SmartValue;
            value.item = newItem;
            value.weight = newItem != null ? 16.67f : 0f;
            value.ValidateRewards();
            this.ValueEntry.SmartValue = value;
        }

        var weightRect = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.BeginChangeCheck();
        var newWeight = EditorGUI.Slider(weightRect, "Weight", this.ValueEntry.SmartValue.weight, 0, 100);
        if (EditorGUI.EndChangeCheck())
        {
            var value = this.ValueEntry.SmartValue;
            value.weight = newWeight;
            this.ValueEntry.SmartValue = value;
        }

        if (item != null && item.rewardsToGive.Count > 0)
        {
            showRewards = EditorGUILayout.Foldout(showRewards, "Rewards");
            if (showRewards)
            {
                EditorGUI.indentLevel++;

                foreach (var reward in item.rewardsToGive)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    var rewardIcon = GUIHelper.GetAssetThumbnail(reward.itemSprite, reward.GetType(), true);
                    var iconRect = EditorGUILayout.GetControlRect(false, 40);
                    GUI.DrawTexture(iconRect.AlignLeft(40), rewardIcon, ScaleMode.ScaleToFit);

                    var labelRect = iconRect.AddXMin(45);
                    GUI.Label(labelRect, reward.itemName);

                    if (reward is NumberBasedReward)
                    {
                        EditorGUI.BeginChangeCheck();
                        var currentValue = this.ValueEntry.SmartValue.GetValue<int>(reward);
                        var baseValue = (reward as NumberBasedReward).BaseValue;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Base Value:", baseValue.ToString());
                        var newValue = EditorGUILayout.IntField("Value:", currentValue);
                        EditorGUILayout.EndHorizontal();

                        if (EditorGUI.EndChangeCheck())
                        {
                            var value = this.ValueEntry.SmartValue;
                            value.SetValue(reward, newValue);
                            this.ValueEntry.SmartValue = value;
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}