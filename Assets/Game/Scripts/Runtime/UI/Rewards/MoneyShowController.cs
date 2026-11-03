using Game.Collectable;
using Game.Utilities;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game.UI.Rewards
{
    public class MoneyShowController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;
        private readonly GoldWallet goldWallet = new GoldWallet();
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            UpdateMoneyDisplay();

            MessageBroker.Default
                .Receive<GameOverMessage>()
                .Subscribe(_ => UpdateMoneyDisplay())
                .AddTo(disposables);
        }

        private void UpdateMoneyDisplay()
        {
            moneyText.text = goldWallet.Gold.ToString();
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }

}