using System;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class InputDialogWindow : EditorWindow
    {
        private int contentCount = 1;
        public System.Action<int> OnConfirm;

        public static void ShowWindow(System.Action<int> onConfirm)
        {
            var window = CreateInstance<InputDialogWindow>();
            window.OnConfirm = onConfirm;

            var editorWindowPosition = EditorGUIUtility.GetMainWindowPosition();

            var width = 300;
            var height = 150;

            window.position = new Rect(
                editorWindowPosition.x + (editorWindowPosition.width - width) / 2,
                editorWindowPosition.y + (editorWindowPosition.height - height) / 2,
                width,
                height
            );

            window.ShowModalUtility();
        }
        private void OnGUI()
        {
            try
            {
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("Enter number of contents to add:");

                contentCount = EditorGUILayout.IntField(contentCount);
                contentCount = Mathf.Max(1, contentCount);

                EditorGUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add"))
                {
                    OnConfirm?.Invoke(contentCount);
                    this.Close();
                }

                if (GUILayout.Button("Cancel"))
                {
                    this.Close();
                }

                // End horizontal layout
                EditorGUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in InputDialogWindow.OnGUI: {ex.Message}\n{ex.StackTrace}");
            }
        }

    }
}