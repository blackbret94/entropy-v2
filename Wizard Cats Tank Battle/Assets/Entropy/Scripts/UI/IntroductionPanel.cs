using System;
using CBS;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.SaveLoad;
using WebSocketSharp;

namespace Vashta.Entropy.UI
{
    public class IntroductionPanel: GamePanel
    {
        /// <summary>
		/// Settings: input field for the player name.
		/// </summary>
		public InputField nameField;

        public CatNameGenerator CatNameGenerator;
        
        private IProfile ProfileModule { get; set; }

        public void Init()
        {
            ProfileModule = CBSModule.Get<CBSProfile>();
            ProfileModule.GetAccountInfo(OnAccountInfoGetted);
        }
        
        private void OnAccountInfoGetted(CBSGetAccountInfoResult result)
        {
            if (result.IsSuccess)
            {
                if (result.DisplayName.IsNullOrEmpty())
                {
                    OpenPanel();
                    nameField.text = CatNameGenerator.GetRandomName();
                }
            }
        }

        public void SaveChanges()
        {
            string username = nameField.text;
            if (username.IsNullOrEmpty())
                username = CatNameGenerator.GetRandomName();
                    
            ProfileModule.UpdateUserName(username);
            ClosePanel();
        }
    }
}