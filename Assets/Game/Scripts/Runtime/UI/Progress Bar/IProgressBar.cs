using DG.Tweening;

namespace Game.UI.Progressbar
{
    public interface IProgressBar
    {
        void Stop();
        void PlayTransitionAnimation(int waveNumbe);
    }
}