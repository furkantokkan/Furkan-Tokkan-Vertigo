#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;
using UnityEditor;

public abstract class AbstractValidator<TAttribute, TValue> : AttributeValidator<TAttribute, TValue>
    where TAttribute : System.Attribute
{
    protected abstract System.Type ValidationType { get; }

    protected abstract void ValidateTarget(ValidationResult result, ScriptableObject target);

    protected override void Validate(ValidationResult result)
    {
        if (!ValidateComponents(result))
        {
            return;
        }

        var targetObject = GetTargetObject();
        if (!ValidateTargetObject(result, targetObject))
        {
            return;
        }

        ValidateTarget(result, targetObject);
    }

    private bool ValidateComponents(ValidationResult result)
    {
        if (result == null || Property == null || Attribute == null)
        {
            Debug.LogError("Validation components are null");
            return false;
        }

        return true;
    }

    protected ScriptableObject GetTargetObject()
    {
        try
        {
            if (Property?.Tree?.WeakTargets == null || Property.Tree.WeakTargets.Count == 0)
            {
                return null;
            }

            return Property.Tree.WeakTargets[0] as ScriptableObject;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error getting target object: {ex.Message}");
            return null;
        }
    }

    private bool ValidateTargetObject(ValidationResult result, ScriptableObject targetObject)
    {
        if (targetObject == null)
        {
            result.AddWarning($"Cannot find {ValidationType?.Name ?? "unknown"} object.");
            return false;
        }

        if (!ValidationType.IsInstanceOfType(targetObject))
        {
            result.AddError($"Object is not of type {ValidationType?.Name ?? "unknown"}");
            return false;
        }

        return true;
    }

    protected void SaveChanges(UnityEngine.Object obj)
    {
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssets();
    }

    protected void RenameAsset(ScriptableObject obj, string newName)
    {
        try
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.RenameAsset(assetPath, newName);
                SaveChanges(obj);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error renaming asset: {ex.Message}");
        }
    }
}
#endif