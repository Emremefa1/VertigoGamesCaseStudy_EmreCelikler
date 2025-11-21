using UnityEngine;

namespace WheelGame.Core
{
    /// <summary>
    /// Minimal save system using PlayerPrefs for demo.
    /// Replace with robust persistence in production.
    /// </summary>
    public class SaveSystem
    {
        private const string BANK_KEY = "WG_BANKED";

        public void SaveBank(int value)
        {
            PlayerPrefs.SetInt(BANK_KEY, value);
            PlayerPrefs.Save();
        }

        public int LoadBank()
        {
            return PlayerPrefs.GetInt(BANK_KEY, 0);
        }
    }
}
