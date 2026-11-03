using System.Collections.Generic;
using System.Linq;
using Game.Collectable;
using Game.Editor;
using Game.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Rewards
{
    public class RewardPanelController : MonoBehaviour
    {
        [SerializeField] private RewardItemContent rewardItemPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private Button exitButton;

        private RewardItemPoolManager poolManager;
        private readonly Dictionary<AbstractReward, int> rewardCounts = new Dictionary<AbstractReward, int>();
        private readonly GoldWallet goldWallet = new GoldWallet();
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            InitializePool();
            SubscribeToEvents();
            exitButton.onClick.AddListener(() => RequestExitPopup());
        }
        private void OnValidate()
        {
            exitButton = GetComponentInChildren<Button>();
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
                .Receive<GameOverMessage>()
                .Subscribe(_ => ResetRewards())
                .AddTo(disposables);

            MessageBroker.Default
                .Receive<ExitConfirmedMessage>()
                .Subscribe(_ => HandleOnSelectExit())
                .AddTo(disposables);
        }

        private void RequestExitPopup()
        {
            MessageBroker.Default.Publish(new ExitButtonClickedMessage());
        }
        private void HandleOnSelectExit()
        {
            SaveGoldPoints();
            MessageBroker.Default.Publish(new GameOverMessage());
        }
        private void SaveGoldPoints()
        {
            foreach (var kvp in rewardCounts)
            {
                var reward = kvp.Key;
                if (reward != null && reward.RewardType == RewardType.GoldPoint)
                {
                    goldWallet.Add(kvp.Value);
                    Debug.Log($"Saved Gold: {goldWallet.Gold} (Added: {kvp.Value})");
                    break;
                }
            }
        }

        private void OnRewardGiven(RewardGivenMessage message)
        {
            if (message.Slot == null || message.Slot.item.rewardsToGive.Count == 0) return;

            foreach (var reward in message.Slot.item.rewardsToGive)
            {
                if (reward == null) continue;

                int rewardAmount = message.Slot.GetAmount(reward);

                if (rewardCounts.ContainsKey(reward))
                {
                    rewardCounts[reward] += rewardAmount;
                }
                else
                {
                    rewardCounts.Add(reward, rewardAmount);
                }

                poolManager.GetOrUpdateExisting(reward.itemSprite, rewardCounts[reward]);
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