using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.Rewards
{
    public class RewardItemPoolManager : ObjectPoolManager<RewardItemContent>
    {
        private readonly Transform container;
        private readonly List<RewardItemContent> activeItems = new List<RewardItemContent>();

        public IReadOnlyList<RewardItemContent> ActiveItems => activeItems;

        public RewardItemPoolManager(RewardItemContent prefab, Transform container, int defaultCapacity = 10)
            : base(prefab, defaultCapacity)
        {
            this.container = container;
        }

        protected override RewardItemContent CreateItem()
        {
            var item = base.CreateItem();
            item.transform.SetParent(container, false);
            return item;
        }

        protected override void OnGetItem(RewardItemContent item)
        {
            base.OnGetItem(item);
            activeItems.Add(item);
        }

        protected override void OnReleaseItem(RewardItemContent item)
        {
            base.OnReleaseItem(item);
            activeItems.Remove(item);
        }

        public RewardItemContent GetOrUpdateExisting(Sprite icon, int amount)
        {
            var existingItem = activeItems.FirstOrDefault(item => item.MatchesIcon(icon));

            if (existingItem != null)
            {
                existingItem.UpdateAmount(amount);
                return existingItem;
            }

            var newItem = Get();
            newItem.Setup(icon, amount);
            return newItem;
        }
    }
}
