using System;
using System.Collections;
using Game.Collectable;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class RewardCreationController : AbstractCreator
    {
        public override void CreateToolbar(OdinMenuItem selected, EditorMenuState state)
        {
            DefaultContent(selected);
        }
        public override void DefaultContent(OdinMenuItem selected)
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Reward")))
            {
                ShowTypeSelector<AbstractReward>(selectedType =>
                {
                    CreateWithFilePanel(
                        selectedType,
                        EditorConst.Reward_TITLE,
                        EditorConst.REWARD_PATH,
                        asset =>
                        {
                            var reward = (AbstractReward)asset;
                            if (reward != null)
                            {
                                EnumGenerator.Generate(
                                    EditorConst.REWARD_TYPE_NAME,
                                    EditorConst.REWARD_TYPE_PATH,
                                    reward.name,
                                    EnumGenerator.GenerateMode.Append,
                                    (x) =>
                                    {
                                        reward.itemName = reward.name;
                                        reward.rewardTypeId = EnumGenerator.GetExistingElements(EditorConst.REWARD_TYPE_NAME, EditorConst.REWARD_TYPE_PATH)[x[0]];

                                        EditorUtility.SetDirty(reward);
                                        AssetDatabase.SaveAssets();
                                    });
                            }
                        });
                });
            }
        }
    }
}
