using UnityEngine;

namespace Game.Utilities
{
    // The player's saved gold; the only code that reads or writes its PlayerPrefs key.
    public class GoldWallet
    {
        private const string GOLD_KEY = "Gold";

        public int Gold => PlayerPrefs.GetInt(GOLD_KEY, 0);

        public void Add(int amount)
        {
            PlayerPrefs.SetInt(GOLD_KEY, Gold + amount);
            PlayerPrefs.Save();
        }
    }
}
