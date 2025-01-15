using DG.Tweening;
using Game.UI.Progressbar;
using Game.Utilities;
using UniRx;
using UnityEngine.UI;
using UnityEngine;

public class DOTweenProgressBar : IProgressBar
{
    private readonly RawImage progressImage;
    private readonly ProgressBarSettings settings;
    private Sequence currentSequence;

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
        SetPosition(settings.LeftPoint);

        currentSequence = DOTween.Sequence();

        var firstMove = DOTween.To(
            () => progressImage.uvRect.x,
            SetPosition,
            settings.FirstStopPoint,
            settings.LeftMoveDuration
        ).SetEase(settings.LeftEase);

        firstMove.OnComplete(() => MessageBroker.Default.Publish(GameConst.PROGRESS_FIRST_STOP));

        currentSequence.Append(firstMove);

        currentSequence.Append(
            DOTween.To(
                () => progressImage.uvRect.x,
                SetPosition,
                settings.SecondStopPoint,
                settings.LeftMoveDuration
            ).SetEase(settings.LeftEase)
        );

        currentSequence.Append(
            DOTween.To(
                () => progressImage.uvRect.x,
                SetPosition,
                settings.RightPoint,
                settings.RightMoveDuration
            ).SetEase(settings.RightEase)
        );

        currentSequence.AppendCallback(() => SetPosition(settings.LeftPoint));
    }

    private void SetPosition(float x)
    {
        progressImage.uvRect = new Rect(x, 0, 1, 1);
    }
}