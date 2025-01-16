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
        [SerializeField] private GameObject slotPrefab;

        [Header("Spin Settings")]
        [SerializeField] private float spinDuration = 3f;
        [SerializeField] private int spinRotations = 5;

        [Header("Spin Ease Settings")]
        [SerializeField] private Ease spinEasePhase1 = Ease.InQuad;    
        [SerializeField] private Ease spinEasePhase2 = Ease.InOutSine; 
        [SerializeField] private Ease spinEasePhase3 = Ease.OutCirc; 

        [Header("Spin Phase Durations")]
        [Range(0.1f, 0.5f)]
        [SerializeField] private float phase1Duration = 0.3f; 
        [Range(0.3f, 0.7f)]
        [SerializeField] private float phase2Duration = 0.5f;
        [Range(0.1f, 0.4f)]
        [SerializeField] private float phase3Duration = 0.2f; 

        [Header("Slot Settings")]
        [SerializeField] private float slotDistance = 140f;
        [SerializeField] private float textVerticalOffset = -35f;
        [SerializeField] private float imageVerticalOffset = 5f;
        [SerializeField] private Vector2 slotSize = new Vector2(50, 50);
        [SerializeField] private Vector2 imageSize = new Vector2(35, 35);
        [SerializeField] private Vector2 textSize = new Vector2(30, 12);
        [SerializeField] private float textScale = 0.8f;

        public readonly float AnglePerSlot = 360f / 8;
        public readonly float OffsetAngle = 0;
        public readonly int SlotCount = 8;

        // Properties
        public Box Box => box;
        public GameObject SlotPrefab => slotPrefab;
        public float ImageVerticalOffset => imageVerticalOffset;
        public float TextVerticalOffset => textVerticalOffset;
        public float SpinDuration => spinDuration;
        public int SpinRotations => spinRotations;
        public Ease SpinEasePhase1 => spinEasePhase1;
        public Ease SpinEasePhase2 => spinEasePhase2;
        public Ease SpinEasePhase3 => spinEasePhase3;
        public float Phase1Duration => phase1Duration;
        public float Phase2Duration => phase2Duration;
        public float Phase3Duration => phase3Duration;
        public float SlotDistance => slotDistance;
        public Vector2 SlotSize => slotSize;
        public Vector2 ImageSize => imageSize;
        public Vector2 TextSize => textSize;
        public float TextScale => textScale;

        private void OnValidate()
        {
            ValidateSlotSettings();
            ValidateSpinDurations();
        }

        private void ValidateSlotSettings()
        {
            if (slotDistance < 100)
                slotDistance = 100f;

            if (textScale <= 0)
                textScale = 0.1f;
        }

        private void ValidateSpinDurations()
        {
            float totalDuration = phase1Duration + phase2Duration + phase3Duration;
            if (totalDuration != 1f)
            {
                float ratio = 1f / totalDuration;
                phase1Duration *= ratio;
                phase2Duration *= ratio;
                phase3Duration *= ratio;
            }
        }
    }
}