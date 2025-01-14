using Game.Collectable;
using Sirenix.Serialization;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class WheelSlot
{
    [Range(0, 100)]
    public float weight;

    public WheelItem item;

    [OdinSerialize]
    private Dictionary<AbstractReward, object> instanceValues = new Dictionary<AbstractReward, object>();

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
            Debug.LogWarning($"Unexpected value type for NumberBasedReward: {value?.GetType()}. Expected int or float.");
            instanceValues[reward] = value;
        }

    }

    public void ValidateRewards()
    {
        if (item == null)
        {
            instanceValues.Clear();
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
    }

    public bool HasReward(AbstractReward reward)
    {
        return reward != null && instanceValues.ContainsKey(reward);
    }

    public object GetRawValue(AbstractReward reward)
    {
        return reward != null && instanceValues.TryGetValue(reward, out var value) ? value : null;
    }
}