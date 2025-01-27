using System.Collections.Generic;
using System.Linq;
using Game.Collectable;
using Game.Editor;
using Game.UI.Popup;
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
        private readonly Dictionary<AbstractReward, float> rewardCounts = new Dictionary<AbstractReward, float>();
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
                .Receive<string>()
                .Where(msg => msg == GameConst.Events.GAME_OVER)
                .Subscribe(_ => ResetRewards())
                .AddTo(disposables);

            MessageBroker.Default
                .Receive<string>()
                .Where(msg => msg == GameConst.Events.EXIT_CONFIRMED)
                .Subscribe(_ => HandleOnSelectExit())
                .AddTo(disposables);
        }

        private void RequestExitPopup()
        {
            MessageBroker.Default.Publish(GameConst.Events.EXIT_BUTTON_CLICKED);
        }
        private void HandleOnSelectExit()
        {
            SaveGoldPoints();
            MessageBroker.Default.Publish(GameConst.Events.GAME_OVER);
        }
        private void SaveGoldPoints()
        {
            foreach (var kvp in rewardCounts)
            {
                var reward = kvp.Key;
                if (reward != null && reward.RewardType == RewardType.GoldPoint)
                {
                    int currentGold = PlayerPrefs.GetInt(GameConst.GOLD_KEY, 0);
                    int newGold = currentGold + Mathf.RoundToInt(kvp.Value);
                    PlayerPrefs.SetInt(GameConst.GOLD_KEY, newGold);
                    PlayerPrefs.Save();

                    Debug.Log($"Saved Gold: {newGold} (Previous: {currentGold}, Added: {kvp.Value})");
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
                poolManager.GetOrUpdateExisting(reward.itemSprite, displayAmount);
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