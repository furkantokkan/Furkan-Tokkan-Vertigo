using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Game.UI.Progressbar;
using Game.Utilities;

public class DOTweenProgressBar : IProgressBar
{
    private readonly RawImage progressImage;
    private readonly ProgressBarSettings settings;
    private Sequence currentSequence;

    private bool midPointReached;

    public DOTweenProgressBar(RawImage progressImage, ProgressBarSettings settings)
    {
        this.progressImage = progressImage;
        this.settings = settings;
    }

    public void Stop()
    {
        if (currentSequence != null && currentSequence.IsPlaying())
        {
            currentSequence.Kill();
            currentSequence = null;
        }
    }

    public void PlayTransitionAnimation(int waveNumber)
    {
        Stop();
        midPointReached = false;

        SetPosition(settings.LeftPoint);

        currentSequence = DOTween.Sequence();

        currentSequence.Append(
            DOTween.To(
                () => progressImage.uvRect.x,
                SetPosition,
                settings.FirstStopPoint,
                settings.LeftMoveDuration
            ).SetEase(settings.LeftEase)
        );

        currentSequence.Append(
            DOTween.To(
                () => progressImage.uvRect.x,
                (x) =>
                {
                    SetPosition(x);
                    CheckMidPoint(x);
                },
                settings.JumpPoint,
                settings.LeftMoveDuration
            ).SetEase(settings.LeftEase)
        );

        currentSequence.Append(
            DOTween.To(
                () => progressImage.uvRect.x,
                (x) =>
                {
                    SetPosition(x);
                    CheckMidPoint(x);
                },
                settings.RightPoint,
                settings.RightMoveDuration
            ).SetEase(settings.RightEase)
        );

        currentSequence.AppendCallback(() =>
        {
            SetPosition(settings.LeftPoint);
            midPointReached = false;
        });
    }

    private void SetPosition(float x)
    {
        progressImage.uvRect = new Rect(x, 0, 1, 1);
    }

    private void CheckMidPoint(float currentPosition)
    {
        if (!midPointReached && currentPosition >= settings.MidPoint)
        {
            midPointReached = true;
            MessageBroker.Default.Publish(GameConst.PROGRESS_MID_POINT);
        }
    }
}