using Codice.Client.BaseCommands.Merge.Xml;
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
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            UpdateMoneyDisplay();

            MessageBroker.Default
                .Receive<string>()
                .Where(msg => msg == GameConst.Events.GAME_OVER)
                .Subscribe(_ => SaveAndUpdateMoney())
                .AddTo(disposables);
        }

        private void UpdateMoneyDisplay()
        {
            int currentMoney = PlayerPrefs.GetInt(GameConst.GOLD_KEY, 0);
            moneyText.text = currentMoney.ToString();
        }

        private void SaveAndUpdateMoney()
        {
            int currentMoney = PlayerPrefs.GetInt(GameConst.GOLD_KEY, 0);
            PlayerPrefs.SetInt(GameConst.GOLD_KEY, currentMoney);
            PlayerPrefs.Save();
            UpdateMoneyDisplay();
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }

}