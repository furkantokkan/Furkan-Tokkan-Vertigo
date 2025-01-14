using Game.Boxes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class BoxCreationController : AbstractCreator
    {
        public override void CreateToolbar(OdinMenuItem selected, EditorMenuState state)
        {
            bool boxSelected = state == EditorMenuState.BoxSelected;
            bool boxContentSelected = state == EditorMenuState.BoxContentSelected;

            if (boxSelected)
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Content")))
                {
                    AddNewBoxContent(selected.Value as Box);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove All Contents")))
                {
                    RemoveAllContents(selected.Value as Box);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove Box")))
                {
                    RemoveSelectedItem(selected);
                }
            }
            else if (boxContentSelected)
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove Content")))
                {
                    RemoveSelectedItem(selected);
                }
            }
            else
            {
                DefaultContent(selected);

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove All Boxes")))
                {
                    RemoveAllBoxes();
                }
            }
        }

        public override void DefaultContent(OdinMenuItem selected)
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Box")))
            {
                string title = EditorConst.BOX_TITLE;
                string path = EditorConst.BOX_PATH;
                CreateWithFilePanel<Box>(title, path, (x) => 
                {
                    x.boxName = x.name;
                });
            }
        }

        private void AddNewBoxContent(Box clickedBox)
        {
            if (clickedBox == null)
            {
                Debug.LogError("Clicked Box is null.");
                return;
            }

            void WindowConfirm(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    BoxContent newContent = ScriptableObject.CreateInstance<BoxContent>();
                    string newContentName = $"{clickedBox.boxName} Content {clickedBox.contents.Count + 1}";
                    newContent.name = newContentName;
                    newContent.contentName = newContentName;
                    newContent.description = string.Empty;

                    AssetDatabase.AddObjectToAsset(newContent, clickedBox);
                    clickedBox.contents.Add(newContent);
                    clickedBox.UpdateContents();
                }

                EditorUtility.SetDirty(clickedBox);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = clickedBox;

                Debug.Log($"{count} new BoxContent(s) added to '{clickedBox.boxName}'.");
            }

            InputDialogWindow.ShowWindow(WindowConfirm);
        }

        private void RemoveSelectedItem(OdinMenuItem selected)
        {
            if (selected == null)
                return;

            if (selected.Value is Box box)
            {
                string path = AssetDatabase.GetAssetPath(box);
                AssetDatabase.DeleteAsset(path);
                Debug.Log($"Box '{box.boxName}' removed.");
            }
            else if (selected.Value is BoxContent boxContent)
            {
                var parentBox = GetParentBox(selected);
                if (parentBox != null)
                {
                    parentBox.contents.Remove(boxContent);
                    AssetDatabase.RemoveObjectFromAsset(boxContent);
                    EditorUtility.SetDirty(parentBox);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"BoxContent '{boxContent.contentName}' removed from '{parentBox.boxName}'.");
                }
            }

            Selection.activeObject = null;
        }

        public void RemoveAllContents(Box box)
        {
            if (box == null)
                return;

            var contentsToRemove = box.contents.ToList();
            foreach (var content in contentsToRemove)
            {
                box.contents.Remove(content);
                AssetDatabase.RemoveObjectFromAsset(content);
            }
            EditorUtility.SetDirty(box);
            AssetDatabase.SaveAssets();
            Debug.Log($"All BoxContents removed from '{box.boxName}'.");
        }

        public void RemoveAllBoxes()
        {
            var boxGuids = AssetDatabase.FindAssets("t:Box", new[] { EditorConst.BOX_PATH });
            foreach (var guid in boxGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("All Boxes removed.");
            Selection.activeObject = null;
        }

        private Box GetParentBox(OdinMenuItem selected)
        {
            var parentItem = selected.Parent;
            if (parentItem != null && parentItem.Value is Box parentBox)
            {
                return parentBox;
            }
            return null;
        }
    }
}
