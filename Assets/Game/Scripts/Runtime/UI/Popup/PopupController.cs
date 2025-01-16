using Game.UI.Popup;
using Game.Utilities;
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
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            MessageBroker.Default.Receive<RewardGivenMessage>()
                .Subscribe(HandleRewardMessage)
                .AddTo(disposables);

            MessageBroker.Default.Receive<BombGivenMessage>()
                .Subscribe(HandleBombMessage)
                .AddTo(disposables);

            bombPopup.onGiveUp = () =>
            {
                MessageBroker.Default.Publish(GameConst.Events.GAME_OVER);
            };

            bombPopup.onRevive = () =>
            {
                MessageBroker.Default.Publish(GameConst.Events.PLAYER_REVIVED);
            };
        }

        private void HandleRewardMessage(RewardGivenMessage message)
        {
            if (bombPopup.gameObject.activeInHierarchy) return;
            rewardPopup.SetContent(message.Title, message.Message, message.Item.ItemSprite);
            rewardPopup.Show();
        }

        private void HandleBombMessage(BombGivenMessage message)
        {
            if (rewardPopup.gameObject.activeInHierarchy) return;
            bombPopup.SetContent(message.Title, message.Message);
            bombPopup.Show();
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }
}
