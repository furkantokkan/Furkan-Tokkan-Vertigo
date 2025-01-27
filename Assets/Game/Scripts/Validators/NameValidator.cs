#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;
using Game.Boxes;
using Game.Collectable;
using UnityEditor;
using System.Collections.Generic;
using System;

[assembly: RegisterValidator(typeof(NameValidator))]

public class NameValidator : AbstractValidator<NameCheckAttribute, string>
{
    private static readonly Dictionary<Type, INameHandler> nameHandlers = new Dictionary<Type, INameHandler>
    {
        { typeof(AbstractCollectable), new CollectableNameHandler() }
    };

    protected override Type ValidationType => Attribute?.type;

    protected override void ValidateTarget(ValidationResult result, ScriptableObject target)
    {
        if (!nameHandlers.ContainsKey(ValidationType))
        {
            result.AddError($"No name handler found for type: {ValidationType}");
            return;
        }

        try
        {
            var handler = nameHandlers[ValidationType];
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
            result.AddError($"Error validating name: {ex.Message}");
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

    public interface INameHandler
    {
        string GetName(ScriptableObject obj);
        void SetName(ScriptableObject obj, string newName);
    }

    private class CollectableNameHandler : INameHandler
    {
        public string GetName(ScriptableObject obj) => (obj as AbstractCollectable)?.itemName;
        public void SetName(ScriptableObject obj, string newName)
        {
            if (obj is AbstractCollectable collectable)
                collectable.itemName = newName;
        }
    }
}
#endif