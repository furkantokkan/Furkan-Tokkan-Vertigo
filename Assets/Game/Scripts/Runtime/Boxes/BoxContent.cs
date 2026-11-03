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
        private const int SAFE_WAVE_INTERVAL = 5;
        private const int SUPER_WAVE_INTERVAL = 30;

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
        private string WaveTypeName => WaveType.ToString().ToUpperInvariant();

        public WaveType WaveType
        {
            get
            {
                if (waveIndex > 0 && waveIndex % SUPER_WAVE_INTERVAL == 0) return WaveType.Super;
                if (waveIndex > 0 && waveIndex % SAFE_WAVE_INTERVAL == 0) return WaveType.Safe;
                return WaveType.Normal;
            }
        }

        [VerticalGroup("Wave")]
        [HideLabel]
        [SerializeField]
        public BoxContentItems BoxContentItems = new BoxContentItems();

        private Color WaveTypeColor => WaveType switch
        {
            WaveType.Super => new Color(1f, 0.78f, 0.25f),
            WaveType.Safe => new Color(0.45f, 0.9f, 0.45f),
            _ => Color.white
        };

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
