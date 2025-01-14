using UnityEngine;
using Sirenix.OdinInspector;
using Game.Collectable;
using UnityEditor;
using System.Linq;

namespace Game.Boxes
{
    [CreateAssetMenu(fileName = "NewBoxContent", menuName = "Boxes/BoxContent")]
    public class BoxContent : SerializedScriptableObject
    {
        [BoxGroup("Content Settings")]
        [Title("Content Details")]
        public string contentName;

        [BoxGroup("Content Settings")]
        [TextArea]
        public string description;

        private Box box;

        [BoxGroup("Content Settings")]
        [PropertyTooltip("Wave index in the box (1-based)")]
        [ReadOnly, ShowInInspector]
        private int waveIndex;

        [BoxGroup("Content Settings")]
        [ShowIf("isSuperBox")]
        [InfoBox("This is a Super Wave", InfoMessageType.Info)]
        [ReadOnly]
        public bool isSuperBox => waveIndex > 0 && (waveIndex + 1) % 30 == 0;

        [ReadOnly]
        [BoxGroup("Content Settings")]
        [ShowIf("isSafeBox")]
        [InfoBox("This is a Safe Wave", InfoMessageType.Info)]
        public bool isSafeBox => waveIndex > 0 && (waveIndex + 1) % 5 == 0;

        [TitleGroup("Slot Settings")]
        [HideLabel]
        public BoxContentItems BoxContentItems;

        public void Initialize(Dependency dep)
        {
            this.box = dep.box;
            this.waveIndex = dep.waveIndex;
            BoxContentItems = new BoxContentItems(box);
        }

        public struct Dependency
        {
            public Box box;
            public int waveIndex;

            public Dependency(Box box, int waveIndex)
            {
                this.box = box;
                this.waveIndex = waveIndex;
            }
        }
    }
}