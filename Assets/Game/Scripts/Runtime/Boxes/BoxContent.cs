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
        [HorizontalGroup("Wave/Header"), LabelText("Name"), LabelWidth(40)]
        public string contentName;

        [HorizontalGroup("Wave/Header"), LabelWidth(75)]
        public string description;

        [HorizontalGroup("Wave/Header", Width = 90), LabelText("Index"), LabelWidth(40)]
        [PropertyTooltip("Position of this wave in the box (0-based)")]
        [ReadOnly, SerializeField]
        private int waveIndex;

        [HorizontalGroup("Wave/Header", Width = 60)]
        [ShowInInspector, HideLabel, DisplayAsString, GUIColor(nameof(WaveTypeColor))]
        private string WaveTypeName => isSuperBox ? "SUPER" : isSafeBox ? "SAFE" : "NORMAL";

        public bool isSuperBox => waveIndex > 0 && (waveIndex) % 30 == 0;

        public bool isSafeBox => waveIndex > 0 && (waveIndex) % 5 == 0;

        [VerticalGroup("Wave")]
        [HideLabel]
        [SerializeField]
        public BoxContentItems BoxContentItems = new BoxContentItems();

        private Color WaveTypeColor => isSuperBox ? new Color(1f, 0.78f, 0.25f)
            : isSafeBox ? new Color(0.45f, 0.9f, 0.45f)
            : Color.white;

        public void Initialize(Dependency dep)
        {
            this.waveIndex = dep.waveIndex;
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
