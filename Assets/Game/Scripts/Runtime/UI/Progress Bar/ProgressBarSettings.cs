using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ProgressBarSettings", menuName = "UI/Progress Bar Settings")]
public class ProgressBarSettings : ScriptableObject
{
    [SerializeField] private float leftPoint = -1.5f;
    [SerializeField] private float firstStopPoint = -0.8f;
    [SerializeField] private float secondStopPoint = -0.3f;
    [SerializeField] private float rightPoint = 0.5f;

    [SerializeField] private float leftMoveDuration = 0.3f;
    [SerializeField] private float rightMoveDuration = 0.7f;
    [SerializeField] private Ease leftEase = Ease.Linear;
    [SerializeField] private Ease rightEase = Ease.Linear;

    public float LeftPoint => leftPoint;
    public float FirstStopPoint => firstStopPoint;
    public float SecondStopPoint => secondStopPoint;
    public float RightPoint => rightPoint;
    public float LeftMoveDuration => leftMoveDuration;
    public float RightMoveDuration => rightMoveDuration;
    public Ease LeftEase => leftEase;
    public Ease RightEase => rightEase;

    private void OnValidate()
    {
        ValidatePositions();
    }

    private void ValidatePositions()
    {
        if (firstStopPoint <= leftPoint)
            firstStopPoint = leftPoint + 0.1f;

        if (secondStopPoint <= firstStopPoint)
            secondStopPoint = firstStopPoint + 0.1f;

        if (rightPoint <= secondStopPoint)
            rightPoint = secondStopPoint + 0.1f;
    }
}