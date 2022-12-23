using TanksMP;
using UnityEngine;
using Vashta.Entropy.UI;

namespace Vashta.Entropy.TanksExtensions
{
    // checks if the player has a name, if they don't, it assigns one
    public class PlayerNameVerification
    {
        private CatNameGenerator _catNameGenerator;

        public PlayerNameVerification(CatNameGenerator catNameGenerator)
        {
            _catNameGenerator = catNameGenerator;
        }
        
        public void VerifyName()
        {
            string playerName = PlayerPrefs.GetString(PrefsKeys.playerName, "");
            if (string.IsNullOrWhiteSpace(playerName))
            {
                PlayerPrefs.SetString(PrefsKeys.playerName, _catNameGenerator.GetRandomName());
                PlayerPrefs.Save();
            }
        }
    }
}