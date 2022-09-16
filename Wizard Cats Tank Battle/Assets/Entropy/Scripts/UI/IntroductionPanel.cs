using System;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.SaveLoad;

namespace Vashta.Entropy.UI
{
    public class IntroductionPanel: GamePanel
    {
        /// <summary>
		/// Settings: input field for the player name.
		/// </summary>
		public InputField nameField;

        private void Start()
        {
            if (PlayerHasName())
            {
                ClosePanel();
                return;
            }
            
            InitNameField();
        }

        private bool PlayerHasName()
        {
            string defaultString = String.Empty;
            string name = PlayerPrefs.GetString(PrefsKeys.playerName, defaultString);
            Debug.Log("Name: " + name);

            return name != defaultString && name != "";
        }

        private void InitNameField()
        {
            CatNameGenerator catNameGenerator = new CatNameGenerator();
            nameField.text = catNameGenerator.GetRandomName();
        }

        public void SaveChanges()
        {
            PlayerPrefs.SetString(PrefsKeys.playerName, nameField.text);
            PlayerPrefs.Save();
        }
    }
}