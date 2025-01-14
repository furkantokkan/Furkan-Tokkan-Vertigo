using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public abstract class AbstractCreator
    {
        public abstract void CreateToolbar(OdinMenuItem selected, EditorMenuState state);

        public abstract void DefaultContent(OdinMenuItem selected);

        protected void ShowTypeSelector<TBase>(Action<Type> onTypeSelected) where TBase : ScriptableObject
        {
            var derivedTypes = TypeCache.GetTypesDerivedFrom<TBase>()
                .Where(t => !t.IsAbstract)
                .OrderBy(t => t.Name);

            var selector = new TypeSelectorV2(
                types: derivedTypes,              
                supportsMultiSelect: false,     
                showNoneItem: false,             
                showCategories: false,            
                preferNamespaces: false           
            );
         

            selector.SelectionChanged += selection =>
            {
                var selectedType = selection.FirstOrDefault();
                if (selectedType != null)
                {
                    onTypeSelected?.Invoke(selectedType);
                }
            };

            selector.ShowInPopup(300);
        }
        protected void CreateWithFilePanel<T>(string title, string path, Action<T> onCreated)
        {
            CreateWithFilePanel(typeof(T), title, path, (x) =>
            {
                if (x is T asset)
                {
                    onCreated?.Invoke(asset);
                }
            });
        }
        protected void CreateWithFilePanel(Type type, string title, string path, Action<object> onCreated)
        {
            path = EditorUtility.SaveFilePanelInProject(
                $"Save {title}",
                $"New {type.Name}",
                "asset",
                $"Please enter a file name to save the {title} to",
                path);

            if (string.IsNullOrEmpty(path))
                return;

            var asset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"New {asset.GetType()} '{asset.name}' created at '{path}'.");
            onCreated?.Invoke(asset);
        }
    }
}
