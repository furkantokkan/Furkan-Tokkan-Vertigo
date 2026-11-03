using Game.Collectable;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WheelSlot
{
    [Range(0, 100)]
    [Tooltip("Probability of this slot appearing on the wheel")]
    public float weight;

    [Tooltip("Reward item to be displayed in the slot")]
    public WheelItem item;

    [SerializeField]
    private List<RewardAmount> rewardAmounts = new List<RewardAmount>();

    public int GetAmount(AbstractReward reward)
    {
        if (reward == null) return 0;

        var rewardAmount = FindRewardAmount(reward);
        return rewardAmount != null ? rewardAmount.amount : GetBaseAmount(reward);
    }

    public void SetAmount(AbstractReward reward, int amount)
    {
        if (reward == null) return;

        var rewardAmount = FindRewardAmount(reward);
        if (rewardAmount != null)
        {
            rewardAmount.amount = amount;
        }
        else
        {
            rewardAmounts.Add(new RewardAmount { reward = reward, amount = amount });
        }
    }

    public void ValidateRewards()
    {
        if (item == null)
        {
            rewardAmounts.Clear();
            return;
        }

        rewardAmounts.RemoveAll(rewardAmount => rewardAmount.reward == null || !item.rewardsToGive.Contains(rewardAmount.reward));

        foreach (var reward in item.rewardsToGive)
        {
            if (reward != null && FindRewardAmount(reward) == null)
            {
                rewardAmounts.Add(new RewardAmount { reward = reward, amount = GetBaseAmount(reward) });
            }
        }
    }

    private RewardAmount FindRewardAmount(AbstractReward reward)
    {
        foreach (var rewardAmount in rewardAmounts)
        {
            if (rewardAmount.reward == reward)
                return rewardAmount;
        }

        return null;
    }

    private static int GetBaseAmount(AbstractReward reward)
    {
        return reward is NumberBasedReward numberReward ? numberReward.BaseValue : 0;
    }

    [Serializable]
    private class RewardAmount
    {
        public AbstractReward reward;
        public int amount;
    }
}
