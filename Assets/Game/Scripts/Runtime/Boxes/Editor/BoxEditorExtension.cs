using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Boxes;
using Game.Collectable;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Boxes.Editor
{
    public class BoxEditorExtension
    {
        [BoxGroup("Reward Settings")]
        [Title("Number Based Reward Ranges")]
        [InfoBox("Different ranges for different wave types, it will be support another types in the future")]
        [SerializeField] private Vector2 normalWaveRange = new Vector2(10f, 100f);

        [BoxGroup("Reward Settings")]
        [SerializeField] private Vector2 safeWaveRange = new Vector2(20f, 150f);

        [BoxGroup("Reward Settings")]
        [SerializeField] private Vector2 superWaveRange = new Vector2(50f, 200f);

        private Box box;

        private string boxName => box.boxName;

        private List<BoxContent> contents => box.contents;
        public BoxEditorExtension(Box box)
        {
            this.box = box;
        }

        #region Editor Buttons
        [BoxGroup("Contents")]
        [Button("Randomize All Contents"), GUIColor(0.3f, 0.8f, 0.3f)]
        private void RandomizeAllContents()
        {
            if (contents == null || contents.Count == 0)
            {
                Debug.LogWarning($"Box '{boxName}' has no contents to randomize!");
                return;
            }

            foreach (var content in contents)
            {
                if (content != null && content.BoxContentItems != null)
                {
                    RandomizeContent(content);
                }
            }

            EditorUtility.SetDirty(box);
            AssetDatabase.SaveAssets();

            Debug.Log($"Randomized all contents for Box '{boxName}'. Total waves processed: {contents.Count}");
        }

        [BoxGroup("Contents")]
        [Button("Equalize All Weights"), GUIColor(0.1f, 0.5f, 0.5f)]
        private void EqualizeAllWeights()
        {
            if (contents == null || contents.Count == 0)
            {
                Debug.LogWarning($"Box '{boxName}' has no contents to equalize!");
                return;
            }

            foreach (var content in contents)
            {
                if (content != null && content.BoxContentItems != null)
                {
                    EqualizeContentWeights(content);
                }
            }

            EditorUtility.SetDirty(box);
            AssetDatabase.SaveAssets();

            Debug.Log($"Equalized all weights for Box '{boxName}'");
        }
        #endregion

        #region Content Management
        private void RandomizeContent(BoxContent content)
        {
            bool isSafeOrSuper = content.isSafeBox || content.isSuperBox;

            var (bombItem, normalItems) = GetItems();
            if (!ValidateItems(bombItem, normalItems, isSafeOrSuper)) return;

            var slots = content.BoxContentItems.Slots;
            System.Random random = new System.Random();

            Vector2 currentRange = GetWaveRange(content);

            FillNormalSlots(slots, normalItems, random, currentRange);

            if (!isSafeOrSuper && bombItem != null)
            {
                AddBombToRandomSlot(slots, bombItem, random, currentRange);
            }

            NormalizeWeights(slots);
            EditorUtility.SetDirty(content);
        }

        private void EqualizeContentWeights(BoxContent content)
        {
            var slots = content.BoxContentItems.Slots;
            int filledSlots = slots.Count(s => s != null && s.item != null);

            if (filledSlots > 0)
            {
                float equalWeight = 100f / filledSlots;

                foreach (var slot in slots)
                {
                    if (slot != null && slot.item != null)
                    {
                        slot.weight = equalWeight;
                    }
                    else if (slot != null)
                    {
                        slot.weight = 0;
                    }
                }

                EditorUtility.SetDirty(content);
            }
        }

        #endregion

        #region Helpers
        private (BombItem bomb, List<WheelItem> normal) GetItems()
        {
            var bombItemGuids = AssetDatabase.FindAssets("t:BombItem");
            var bombItem = bombItemGuids.Length > 0
                ? AssetDatabase.LoadAssetAtPath<BombItem>(AssetDatabase.GUIDToAssetPath(bombItemGuids[0]))
                : null;

            var wheelItemGuids = AssetDatabase.FindAssets("t:WheelItem");
            var normalItems = wheelItemGuids
                .Select(guid => AssetDatabase.LoadAssetAtPath<WheelItem>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(item => item != null && !(item is BombItem))
                .ToList();

            return (bombItem, normalItems);
        }

        private bool ValidateItems(BombItem bombItem, List<WheelItem> normalItems, bool isSafeOrSuper)
        {
            if (normalItems.Count == 0)
            {
                Debug.LogWarning($"No normal items found!");
                return false;
            }

            if (!isSafeOrSuper && bombItem == null)
            {
                Debug.LogWarning("No Bomb item found! Please create one using 'Create Bomb' button.");
                return false;
            }

            return true;
        }

        private Vector2 GetWaveRange(BoxContent content)
        {
            if (content.isSuperBox) return superWaveRange;
            if (content.isSafeBox) return safeWaveRange;
            return normalWaveRange;
        }

        private void FillNormalSlots(WheelSlot[] slots, List<WheelItem> normalItems, System.Random random, Vector2 range)
        {
            if (slots == null || normalItems == null || normalItems.Count == 0)
            {
                Debug.LogWarning("Invalid parameters for FillNormalSlots");
                return;
            }

            foreach (var slot in slots)
            {
                try
                {
                    if (slot == null) continue;

                    int randomIndex = random.Next(normalItems.Count);
                    var selectedItem = normalItems[randomIndex];
                    slot.item = selectedItem;

                    slot.weight = random.Next(10, 31);

                    slot.ValidateRewards();

                    if (slot.item != null && slot.item.rewardsToGive != null)
                    {
                        foreach (var reward in slot.item.rewardsToGive.Where(r => r != null))
                        {
                            if (reward is NumberBasedReward numberReward)
                            {
                                float minValue = Mathf.Max(numberReward.BaseValue * 0.5f, range.x);
                                float maxValue = Mathf.Min(numberReward.BaseValue * 2f, range.y);

                                float randomValue = UnityEngine.Random.Range(minValue, maxValue);

                                slot.SetValue(reward, randomValue);

                                Debug.Log($"Set reward value for {reward.name}: {randomValue} " +
                                        $"(Range: {minValue}-{maxValue}, Base: {numberReward.BaseValue})");
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error while filling slot: {e.Message}\n{e.StackTrace}");
                }
            }

            Debug.Log($"Filled {slots.Count(s => s != null && s.item != null)} slots with normal items");
        }


        private void AddBombToRandomSlot(WheelSlot[] slots, BombItem bombItem, System.Random random, Vector2 range)
        {
            int bombSlotIndex = random.Next(0, slots.Length);
            slots[bombSlotIndex].item = bombItem;
            slots[bombSlotIndex].ValidateRewards();

            if (bombItem.rewardsToGive != null)
            {
                foreach (var reward in bombItem.rewardsToGive)
                {
                    if (reward is NumberBasedReward)
                    {
                        float randomValue = Random.Range(range.x, range.y);
                        slots[bombSlotIndex].SetValue(reward, randomValue);
                    }
                }
            }
        }

        private void NormalizeWeights(WheelSlot[] slots)
        {
            float totalWeight = slots.Sum(s => s?.weight ?? 0);
            if (totalWeight > 0)
            {
                foreach (var slot in slots)
                {
                    if (slot != null && slot.weight > 0)
                    {
                        slot.weight = (slot.weight / totalWeight) * 100;
                    }
                }
            }
        }
        #endregion
    }
}