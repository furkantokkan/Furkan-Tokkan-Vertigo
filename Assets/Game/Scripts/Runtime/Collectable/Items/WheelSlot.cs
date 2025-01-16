using Game.Collectable;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WheelSlot : ISerializationCallbackReceiver
{
    [Range(0, 100)]
    [Tooltip("Probability of this slot appearing on the wheel")]
    public float weight;

    [SerializeField]
    [OnValueChanged(nameof(OnItemChanged))]
    [Tooltip("Reward item to be displayed in the slot")]
    public WheelItem item;

    [SerializeField]
    private List<AbstractReward> serializedRewardKeys = new List<AbstractReward>();

    [SerializeField]
    private List<string> serializedRewardValues = new List<string>();

    [NonSerialized]
    private Dictionary<AbstractReward, object> instanceValues = new Dictionary<AbstractReward, object>();

    public void OnBeforeSerialize()
    {
        serializedRewardKeys.Clear();
        serializedRewardValues.Clear();

        foreach (var kvp in instanceValues)
        {
            if (kvp.Key != null)
            {
                serializedRewardKeys.Add(kvp.Key);
                serializedRewardValues.Add(JsonUtility.ToJson(new SerializedValue { value = kvp.Value.ToString() }));
            }
        }
    }

    public void OnAfterDeserialize()
    {
        instanceValues = new Dictionary<AbstractReward, object>();

        for (int i = 0; i < serializedRewardKeys.Count; i++)
        {
            if (serializedRewardKeys[i] != null && i < serializedRewardValues.Count)
            {
                var serializedValue = JsonUtility.FromJson<SerializedValue>(serializedRewardValues[i]);
                if (float.TryParse(serializedValue.value, out float floatValue))
                {
                    instanceValues[serializedRewardKeys[i]] = floatValue;
                }
            }
        }
    }

    [Serializable]
    private class SerializedValue
    {
        public string value;
    }

#if UNITY_EDITOR
    private void OnItemChanged()
    {
        ValidateRewards();

        var targetObject = UnityEditor.Selection.activeObject;
        if (targetObject != null)
        {
            UnityEditor.EditorUtility.SetDirty(targetObject);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
#endif

    public T GetValue<T>(AbstractReward reward)
    {
        if (reward == null)
        {
            Debug.LogWarning("Reward is null. Returning default value.");
            return default;
        }

        if (!instanceValues.TryGetValue(reward, out var value))
        {
            if (reward is NumberBasedReward numberReward)
            {
                value = numberReward.BaseValue;
                instanceValues[reward] = value;
            }
            else
            {
                Debug.LogWarning($"Reward '{reward}' is not initialized. Returning default value.");
                return default;
            }
        }

        try
        {
            if (typeof(T) == typeof(int) && value is float floatValue)
            {
                return (T)(object)Mathf.RoundToInt(floatValue);
            }
            else if (typeof(T) == typeof(float) && value is int intValue)
            {
                return (T)(object)((float)intValue);
            }
            else if (value is T typedValue)
            {
                return typedValue;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Type conversion failed for reward '{reward}'. Value: {value} ({value?.GetType()}), Target Type: {typeof(T)}. Error: {ex.Message}");
            return default;
        }
    }

    public void SetValue<T>(AbstractReward reward, T value)
    {
        if (reward == null)
        {
            Debug.LogWarning("Cannot set value for a null reward.");
            return;
        }

        if (value is int intValue)
        {
            instanceValues[reward] = (float)intValue;
        }
        else if (value is float floatValue)
        {
            instanceValues[reward] = floatValue;
        }
        else
        {
            instanceValues[reward] = value;
        }

#if UNITY_EDITOR
        var targetObject = UnityEditor.Selection.activeObject;
        if (targetObject != null)
        {
            UnityEditor.EditorUtility.SetDirty(targetObject);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }

    [Button("Validate Rewards")]
    public void ValidateRewards()
    {
        if (item == null)
        {
            instanceValues.Clear();
            serializedRewardKeys.Clear();
            serializedRewardValues.Clear();
            return;
        }

        var keysToRemove = instanceValues.Keys
            .Where(key => key == null || !item.rewardsToGive.Contains(key))
            .ToList();

        foreach (var key in keysToRemove)
        {
            instanceValues.Remove(key);
        }

        foreach (var reward in item.rewardsToGive)
        {
            if (!instanceValues.ContainsKey(reward))
            {
                if (reward is NumberBasedReward numberReward)
                {
                    instanceValues[reward] = (float)numberReward.BaseValue;
                }
            }
        }

#if UNITY_EDITOR
        var targetObject = UnityEditor.Selection.activeObject;
        if (targetObject != null)
        {
            UnityEditor.EditorUtility.SetDirty(targetObject);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }

#if UNITY_EDITOR
    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        if (UnityEditor.EditorApplication.isPlaying) return;

        if (item != null && item.rewardsToGive != null && item.rewardsToGive.Any())
        {
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField("Reward Values", UnityEditor.EditorStyles.boldLabel);

            foreach (var reward in item.rewardsToGive)
            {
                if (reward is NumberBasedReward)
                {
                    var currentValue = GetValue<float>(reward);
                    var newValue = UnityEditor.EditorGUILayout.FloatField(reward.name, currentValue);
                    if (!Mathf.Approximately(currentValue, newValue))
                    {
                        SetValue(reward, newValue);
                    }
                }
            }
        }
    }
#endif
}