#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;
using Game.Boxes;
using Game.Collectable;
using UnityEditor;
using System.Collections.Generic;
using System;

[assembly: RegisterValidator(typeof(NameValidator))]
public class NameValidator : AttributeValidator<NameCheckAttribute, string>
{
    private static readonly Dictionary<Type, INameHandler> nameHandlers = new Dictionary<Type, INameHandler>
    {
        { typeof(Box), new BoxNameHandler() },
        { typeof(AbstractCollectable), new CollectableNameHandler() }
    };

    protected override void Validate(ValidationResult result)
    {
        if (!ValidateSetup(result))
        {
            return;
        }

        var targetObject = GetTargetObject();
        if (targetObject == null)
        {
            result.AddError("Cannot find target object");
            return;
        }

        ValidateName(result, targetObject);
    }

    private bool ValidateSetup(ValidationResult result)
    {
        if (result == null || Property == null || Attribute == null)
        {
            Debug.LogError("Validation components are null");
            return false;
        }

        if (!nameHandlers.ContainsKey(Attribute.type))
        {
            Debug.LogError($"No name handler found for type: {Attribute.type}");
            return false;
        }

        return true;
    }

    private ScriptableObject GetTargetObject()
    {
        try
        {
            if (Property?.Tree?.WeakTargets == null || Property.Tree.WeakTargets.Count == 0)
            {
                return null;
            }

            return Property.Tree.WeakTargets[0] as ScriptableObject;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting target object: {ex.Message}");
            return null;
        }
    }

    private void ValidateName(ValidationResult result, ScriptableObject target)
    {
        try
        {
            var handler = nameHandlers[Attribute.type];
            string currentName = handler.GetName(target);

            if (string.IsNullOrEmpty(currentName))
            {
                HandleEmptyName(result, target, handler);
                return;
            }

            if (target.name != currentName)
            {
                HandleNameMismatch(result, target, currentName);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error validating name: {ex.Message}");
        }
    }

    private void HandleEmptyName(ValidationResult result, ScriptableObject obj, INameHandler handler)
    {
        result.AddWarning("Name field is empty.")
            .WithFix(() =>
            {
                try
                {
                    handler.SetName(obj, obj.name);
                    SaveChanges(obj);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error setting name: {ex.Message}");
                }
            });
    }

    private void HandleNameMismatch(ValidationResult result, ScriptableObject obj, string currentName)
    {
        result.AddError($"Asset name '{obj.name}' doesn't match field name '{currentName}'.")
            .WithFix(() =>
            {
                try
                {
                    RenameAsset(obj, currentName);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error renaming asset: {ex.Message}");
                }
            });
    }

    private void RenameAsset(ScriptableObject obj, string newName)
    {
        string assetPath = AssetDatabase.GetAssetPath(obj);
        if (!string.IsNullOrEmpty(assetPath))
        {
            AssetDatabase.RenameAsset(assetPath, newName);
            SaveChanges(obj);
        }
    }

    private void SaveChanges(UnityEngine.Object obj)
    {
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssets();
    }

    public interface INameHandler
    {
        string GetName(ScriptableObject obj);
        void SetName(ScriptableObject obj, string newName);
    }

    class BoxNameHandler : INameHandler
    {
        public string GetName(ScriptableObject obj)
        {
            return (obj as Box)?.boxName;
        }

        public void SetName(ScriptableObject obj, string newName)
        {
            if (obj is Box box)
            {
                box.boxName = newName;
            }
        }
    }
    class CollectableNameHandler : INameHandler
    {
        public string GetName(ScriptableObject obj)
        {
            return (obj as AbstractCollectable)?.ItemName;
        }

        public void SetName(ScriptableObject obj, string newName)
        {
            if (obj is AbstractCollectable collectable)
            {
                collectable.ItemName = newName;
            }
        }
    }
}
#endif