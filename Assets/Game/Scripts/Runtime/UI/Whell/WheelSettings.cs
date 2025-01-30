// WheelSettings.cs
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using Game.Boxes;

namespace Game.UI.Wheel
{
    [CreateAssetMenu(fileName = "WheelSettings", menuName = "UI/Wheel Settings")]
    public class WheelSettings : ScriptableObject
    {
        [Header("References")]
        [SerializeField] private Box box;

        [Header("Slot Prefab")]
        [SerializeField] private WheelObject slotPrefab;

        [Header("Spin Settings")]
        [SerializeField] private float spinDuration = 3f;
        [SerializeField] private int spinRotations = 5;

        [Header("Spin Ease Settings")]
        [SerializeField] private Ease spinEasePhase1 = Ease.InQuad;    
        [SerializeField] private Ease spinEasePhase2 = Ease.InOutSine; 
        [SerializeField] private Ease spinEasePhase3 = Ease.OutCirc; 
        [Header("Slot Settings")]
        [SerializeField] private float slotDistance = 140f;
        [SerializeField] private Vector2 slotSize = new Vector2(50, 50);

        public readonly float AnglePerSlot = 360f / 8;
        public readonly float OffsetAngle = 0;
        public readonly int SlotCount = 8;

        // Properties
        public Box Box => box;
        public WheelObject SlotPrefab => slotPrefab;
        public float SpinDuration => spinDuration;
        public int SpinRotations => spinRotations;
        public Ease SpinEasePhase1 => spinEasePhase1;
        public Ease SpinEasePhase2 => spinEasePhase2;
        public Ease SpinEasePhase3 => spinEasePhase3;
        public float SlotDistance => slotDistance;
        public Vector2 SlotSize => slotSize;

        private void OnValidate()
        {
            ValidateSlotSettings();
        }

        private void ValidateSlotSettings()
        {
            if (slotDistance < 100)
                slotDistance = 100f;

            if (textScale <= 0)
                textScale = 0.1f;
        }
    }
}