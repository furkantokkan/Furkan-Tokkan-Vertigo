using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class ScriptableObjectCreator
    {
        public static void ShowDialog<T>(string defaultDestinationPath, Action<T> onScriptableObjectCreated = null)
            where T : ScriptableObject
        {
            var selector = new ScriptableObjectSelector<T>(defaultDestinationPath, onScriptableObjectCreated);

            if (selector.SelectionTree.EnumerateTree().Count() == 1)
            {
                selector.SelectionTree.EnumerateTree().First().Select();
                selector.SelectionTree.Selection.ConfirmSelection();
            }
            else
            {
                selector.ShowInPopup(200);
            }
        }

        private class ScriptableObjectSelector<T> : OdinSelector<Type> where T : ScriptableObject
        {
            private readonly Action<T> onScriptableObjectCreated;
            private readonly string defaultDestinationPath;

            public ScriptableObjectSelector(string defaultDestinationPath, Action<T> onScriptableObjectCreated = null)
            {
                this.onScriptableObjectCreated = onScriptableObjectCreated;
                this.defaultDestinationPath = defaultDestinationPath;
                this.SelectionConfirmed += this.ShowSaveFileDialog;
            }

            protected override void BuildSelectionTree(OdinMenuTree tree)
            {
                var scriptableObjectTypes = AssemblyUtilities.GetTypes(AssemblyCategory.ProjectSpecific)
                    .Where(x => x.IsClass && !x.IsAbstract && x.InheritsFrom(typeof(T)));

                tree.Selection.SupportsMultiSelect = false;
                tree.Config.DrawSearchToolbar = true;
                tree.AddRange(scriptableObjectTypes, x => x.GetNiceName())
                    .AddThumbnailIcons();
            }

            private void ShowSaveFileDialog(IEnumerable<Type> selection)
            {
                var selectedType = selection.FirstOrDefault();
                if (selectedType == null)
                {
                    Debug.LogError("No valid type selected for ScriptableObject.");
                    return;
                }

                var obj = ScriptableObject.CreateInstance(selectedType) as T;
                if (obj == null)
                {
                    Debug.LogError("Failed to create ScriptableObject instance.");
                    return;
                }

                string dest = this.defaultDestinationPath.TrimEnd('/');
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                    AssetDatabase.Refresh();
                }

                dest = EditorUtility.SaveFilePanel("Save object as", dest, $"New {selectedType.GetNiceName()}", "asset");
                if (!string.IsNullOrEmpty(dest))
                {
                    if (PathUtilities.TryMakeRelative(Application.dataPath, dest, out string relativePath))
                    {
                        relativePath = "Assets/" + relativePath;
                        if (relativePath.Contains("Assets/../"))
                        {
                            relativePath = relativePath.Replace("Assets/../", "Assets/");
                        }

                        string folderPath = Path.GetDirectoryName(relativePath);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                            AssetDatabase.Refresh();
                        }

                        try
                        {
                            AssetDatabase.CreateAsset(obj, relativePath);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            onScriptableObjectCreated?.Invoke(obj);
                            Debug.Log($"ScriptableObject baþarýyla kaydedildi: {relativePath}");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"ScriptableObject kaydedilemedi: {e.Message}");
                            UnityEngine.Object.DestroyImmediate(obj);
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to convert path to a relative path within the Assets folder.");
                        UnityEngine.Object.DestroyImmediate(obj);
                    }
                }
                else
                {
                    Debug.LogWarning("Save iþlemi iptal edildi.");
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }
    }
}
