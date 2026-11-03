using Game.Collectable;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using Game.Boxes.Editor;
#endif

namespace Game.Boxes
{
    public class Box : AbstractCollectable
    {
        [BoxGroup("Box Visuals")]
        [HorizontalGroup("Box Visuals/Row", Width = 75)]
        [PreviewField(75)]
        [Title("Background")]
        public Sprite background;

        [BoxGroup("Box Information")]
        [Title("Box Description")]
        [TextArea(3, 5)]
        public string description;

        [BoxGroup("Contents")]
        [Searchable]
        [TableList(ShowIndexLabels = true, DrawScrollView = false, ShowPaging = true, NumberOfItemsPerPage = 5)]
        [ListDrawerSettings(ShowFoldout = true)]
        [InfoBox("Add box contents here. Each content represents a wave.", InfoMessageType.Info)]
        [SerializeField]
        public List<BoxContent> contents = new List<BoxContent>();

        public WheelSlot GetReward(int currentWave)
        {
            BoxContent waveContent = contents[currentWave];

            if (waveContent == null || waveContent.BoxContentItems == null)
            {
                Debug.LogError($"No items found for wave {currentWave}");
                return null;
            }

            var slots = waveContent.BoxContentItems.Slots;

            float totalWeight = 0f;
            foreach (var slot in slots)
            {
                if (slot.item != null)
                {
                    totalWeight += slot.weight;
                }
            }

            if (totalWeight <= 0)
            {
                Debug.LogError($"Total weight is 0 for wave {currentWave}");
                return null;
            }

            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var slot in slots)
            {
                if (slot.item == null) continue;

                currentWeight += slot.weight;
                if (randomValue <= currentWeight)
                {
                    return slot;
                }
            }

            Debug.LogError($"No valid items found in wave {currentWave}");
            return null;
        }


#if UNITY_EDITOR

        [SerializeField]
        private BoxEditorExtension editorExtension;

        [ExecuteInEditMode]
        private void OnEnable()
        {
            editorExtension = new BoxEditorExtension(this);
        }

        [Button("Refresh Editor Extension")]
        public void RefreshEditorExtension()
        {
            editorExtension = new BoxEditorExtension(this);
        }

        public void UpdateContents()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                if (content != null)
                {
                    content.Initialize(new BoxContent.Dependency(this, i));
                    EditorUtility.SetDirty(content);
                }
            }

            Debug.Log($"Box '{itemName}' contents updated. Count: {contents.Count}");
        }
#endif
    }
}