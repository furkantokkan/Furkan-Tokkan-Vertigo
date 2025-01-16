using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Game.Boxes;

namespace Game.UI.Wheel
{
    [CreateAssetMenu(fileName = "WheelSettings", menuName = "UI/Wheel Settings")]
    public class WheelSettings : ScriptableObject
    {
        [Header("References")]
        [SerializeField] private Box box;

        [Header("GetAllSlotPrefabs")]
        [SerializeField] private GameObject slotPrefab;

        [Header("Spin Settings")]
        [SerializeField] private float spinDuration = 3f;
        [SerializeField] private int spinRotations = 5;
        [SerializeField] private Ease spinEase = Ease.OutQuad;

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


        public Box Box => box;
        public GameObject SlotPrefab => slotPrefab;

        public float ImageVerticalOffset => imageVerticalOffset;
        public float TextVerticalOffset => textVerticalOffset;

        public float SpinDuration => spinDuration;
        public int SpinRotations => spinRotations;
        public Ease SpinEase => spinEase;

        public float SlotDistance => slotDistance;
        public Vector2 SlotSize => slotSize;
        public Vector2 ImageSize => imageSize;
        public Vector2 TextSize => textSize;
        public float TextScale => textScale;

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