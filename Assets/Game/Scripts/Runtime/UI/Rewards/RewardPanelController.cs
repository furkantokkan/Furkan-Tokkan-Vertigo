using System.Collections.Generic;
using System.Linq;
using Game.Collectable;
using Game.UI.Popup;
using Game.Utilities;
using UniRx;
using UnityEngine;

namespace Game.UI.Rewards
{
    public class RewardPanelController : MonoBehaviour
    {
        [SerializeField] private RewardItemContent rewardItemPrefab;
        [SerializeField] private Transform container;

        private RewardItemPoolManager poolManager;
        private readonly Dictionary<AbstractReward, float> rewardCounts = new Dictionary<AbstractReward, float>();
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            InitializePool();
            SubscribeToEvents();
        }

        private void InitializePool()
        {
            poolManager = new RewardItemPoolManager(rewardItemPrefab, container);
        }

        private void SubscribeToEvents()
        {
            MessageBroker.Default
                .Receive<RewardGivenMessage>()
                .Subscribe(OnRewardGiven)
                .AddTo(disposables);

            MessageBroker.Default
                .Receive<string>()
                .Where(msg => msg == GameConst.Events.GAME_OVER)
                .Subscribe(_ => ResetRewards())
                .AddTo(disposables);
        }

        private void OnRewardGiven(RewardGivenMessage message)
        {
            if (message.Slot == null || message.Slot.item.rewardsToGive.Count == 0) return;

            foreach (var reward in message.Slot.item.rewardsToGive)
            {
                if (reward == null) continue;

                float rewardValue = message.Slot.GetValue<float>(reward);

                if (rewardCounts.ContainsKey(reward))
                {
                    rewardCounts[reward] += rewardValue;
                }
                else
                {
                    rewardCounts.Add(reward, rewardValue);
                }

                int displayAmount = Mathf.RoundToInt(rewardCounts[reward]);
                poolManager.GetOrUpdateExisting(reward.ItemSprite, displayAmount);
            }
        }

        private void ResetRewards()
        {
            foreach (var item in poolManager.ActiveItems.ToList())
            {
                poolManager.Release(item);
            }
            rewardCounts.Clear();
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }
}