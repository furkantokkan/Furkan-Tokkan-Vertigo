using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.UI
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] private RewardPopupView rewardPopup;
        [SerializeField] private BombPopupView bombPopup;
        [SerializeField] private DoubleButtonPopupView exitButtonPopup;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            MessageBroker.Default.Receive<RewardGivenMessage>()
                .Subscribe(HandleRewardMessage)
                .AddTo(disposables);

            MessageBroker.Default.Receive<BombGivenMessage>()
                .Subscribe(HandleBombMessage)
                .AddTo(disposables);

            MessageBroker.Default
                .Receive<ExitButtonClickedMessage>()
                .Subscribe(_ => HandleExitButtonPopup())
                .AddTo(disposables);

            bombPopup.onGiveUp = () =>
            {
                MessageBroker.Default.Publish(new GameOverMessage());
            };

            bombPopup.onRevive = () =>
            {
                MessageBroker.Default.Publish(new PlayerRevivedMessage());
            };

            exitButtonPopup.onSelection1 = () =>
            {
                MessageBroker.Default.Publish(new ExitConfirmedMessage());
            };
        }

        private void HandleExitButtonPopup()
        {
            exitButtonPopup.SetContent("Exit", "Are you sure you want to exit?");
            exitButtonPopup.Show();
        }

        private void HandleRewardMessage(RewardGivenMessage message)
        {
            if (bombPopup.gameObject.activeInHierarchy) return;

            var item = message.Slot.item;
            rewardPopup.SetContent($"Reward: {item.itemName}", $"You got {item.itemName} in Wave {message.CurrentWave}", item.itemSprite);
            rewardPopup.Show();
        }

        private void HandleBombMessage(BombGivenMessage message)
        {
            if (rewardPopup.gameObject.activeInHierarchy) return;
            bombPopup.SetContent("Game Over!", "You got the bomb!");
            bombPopup.Show();
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }
}
