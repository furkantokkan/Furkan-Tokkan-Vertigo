using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Game.UI.Progressbar
{
    [CreateAssetMenu(fileName = "ProgressBarSettings", menuName = "UI/Progress Bar Settings")]
    public class ProgressBarSettings : ScriptableObject
    {
        #region Progress Bar Settings
        [FoldoutGroup("Progress Bar")]
        [LabelText("Left Point"), Tooltip("Leftmost position")]
        [SerializeField] private float leftPoint = -1.5f;

        [FoldoutGroup("Progress Bar")]
        [LabelText("Mid Point"), Tooltip("Center position")]
        [SerializeField] private float midPoint = -0.5f;

        [FoldoutGroup("Progress Bar")]
        [LabelText("Right Point"), Tooltip("Rightmost position")]
        [SerializeField] private float rightPoint = 0.5f;

        [FoldoutGroup("Progress Bar")]
        [LabelText("First Stop"), Tooltip("First stop in transition")]
        [SerializeField] private float firstStopPoint = -0.8f;

        [FoldoutGroup("Progress Bar")]
        [LabelText("Jump Point"), Tooltip("Jump position in transition")]
        [SerializeField] private float jumpPoint = -0.3f;

        [FoldoutGroup("Progress Bar Animation")]
        [LabelText("Left Duration"), Tooltip("Duration for left movement")]
        [Range(0.1f, 1f)]
        [SerializeField] private float leftMoveDuration = 0.3f;

        [FoldoutGroup("Progress Bar Animation")]
        [LabelText("Right Duration"), Tooltip("Duration for right movement")]
        [Range(0.1f, 1f)]
        [SerializeField] private float rightMoveDuration = 0.7f;

        [FoldoutGroup("Progress Bar Animation")]
        [LabelText("Left Ease"), Tooltip("Easing for left movement")]
        [SerializeField] private Ease leftEase = Ease.Linear;

        [FoldoutGroup("Progress Bar Animation")]
        [LabelText("Right Ease"), Tooltip("Easing for right movement")]
        [SerializeField] private Ease rightEase = Ease.Linear;
        #endregion

        #region Wave Text Settings
        [FoldoutGroup("Wave Text")]
        [LabelText("Left Wave Count"), Tooltip("Number of waves shown on left")]
        [Range(1, 10)]
        [SerializeField] private int leftWaveCount = 6;

        [FoldoutGroup("Wave Text")]
        [LabelText("Right Wave Count"), Tooltip("Number of waves shown on right")]
        [Range(1, 10)]
        [SerializeField] private int rightWaveCount = 6;

        [FoldoutGroup("Wave Text")]
        [LabelText("Text Spacing"), Tooltip("Space between wave texts")]
        [Range(10f, 100f)]
        [SerializeField] private float textSpacing = 50f;

        [FoldoutGroup("Wave Text Animation")]
        [LabelText("Move Duration"), Tooltip("Duration for wave text movements")]
        [Range(0.1f, 1f)]
        [SerializeField] private float moveDuration = 0.5f;

        [FoldoutGroup("Wave Text Animation")]
        [LabelText("Fade Amount"), Tooltip("Transparency for past waves")]
        [Range(0f, 1f)]
        [SerializeField] private float fadeAmount = 0.5f;

        [FoldoutGroup("Wave Text Animation")]
        [LabelText("Scale Amount"), Tooltip("Scale for past waves")]
        [Range(0.5f, 1f)]
        [SerializeField] private float scaleAmount = 0.9f;
        #endregion

        #region Properties
        public float LeftPoint => leftPoint;
        public float MidPoint => midPoint;
        public float RightPoint => rightPoint;
        public float FirstStopPoint => firstStopPoint;
        public float JumpPoint => jumpPoint;

        public float LeftMoveDuration => leftMoveDuration;
        public float RightMoveDuration => rightMoveDuration;
        public Ease LeftEase => leftEase;
        public Ease RightEase => rightEase;

        public int LeftWaveCount => leftWaveCount;
        public int RightWaveCount => rightWaveCount;
        public float TextSpacing => textSpacing;
        public float MoveDuration => moveDuration;
        public float FadeAmount => fadeAmount;
        public float ScaleAmount => scaleAmount;
        #endregion

        #region Validation
        private void OnValidate()
        {
            ValidatePositions();
        }

        private void ValidatePositions()
        {
            if (leftPoint >= midPoint)
                leftPoint = midPoint - 0.1f;

            if (rightPoint <= midPoint)
                rightPoint = midPoint + 0.1f;

            if (firstStopPoint <= leftPoint)
                firstStopPoint = leftPoint + 0.1f;
            if (firstStopPoint >= midPoint)
                firstStopPoint = midPoint - 0.1f;

            if (jumpPoint <= firstStopPoint)
                jumpPoint = firstStopPoint + 0.1f;
            if (jumpPoint >= midPoint)
                jumpPoint = midPoint - 0.1f;
        }
        #endregion
    }
}