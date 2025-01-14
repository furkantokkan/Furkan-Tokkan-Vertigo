using Game.Collectable;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class ItemCreationController : AbstractCreator
    {
        public override void CreateToolbar(OdinMenuItem selected, EditorMenuState state)
        {
            DefaultContent(selected);

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Bomb")))
            {
                CreateWithFilePanel<BombItem>(
                    "Bomb",
                    EditorConst.ITEM_PATH,
                    asset =>
                    {
                        var bombItem = (BombItem)asset;
                        if (bombItem != null)
                        {
                            EnumGenerator.Generate(
                                EditorConst.ITEM_TYPE_NAME,
                                EditorConst.ITEM_TYPE_PATH,
                                bombItem.name,
                                EnumGenerator.GenerateMode.Append,
                                (x) =>
                                {
                                    bombItem.ItemName = bombItem.name;
                                    bombItem.itemType = (ItemType)x[0];

                                    EditorUtility.SetDirty(bombItem);
                                    AssetDatabase.SaveAssets();
                                });
                        }
                    });
            }
        }

        public override void DefaultContent(OdinMenuItem selected)
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Wheel Item")))
            {
                CreateWithFilePanel<WheelItem>(
                    EditorConst.ITEM_CREATION_MENU,
                    EditorConst.ITEM_PATH,
                    asset =>
                    {
                        var wheelItem = (WheelItem)asset;
                        if (wheelItem != null)
                        {
                            EnumGenerator.Generate(
                                EditorConst.ITEM_TYPE_NAME,
                                EditorConst.ITEM_TYPE_PATH,
                                wheelItem.name,
                                EnumGenerator.GenerateMode.Append,
                                (x) =>
                                {
                                    wheelItem.ItemName = wheelItem.name;
                                    wheelItem.itemType = (ItemType)x[0];

                                    EditorUtility.SetDirty(wheelItem);
                                    AssetDatabase.SaveAssets();
                                });
                        }
                    });
            }
        }
    }
}
