using System.Collections;
using CBS.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class AutoLogin : MonoBehaviour
    {
        public Sprite ImageOff, ImageOn;
        
        public Image Image;
        public LoginForm LoginForm;
        
        private const string AUTO_LOGIN_KEY = "autologin"; // HARDCODED IN AuthContext AS WELL
        private bool _autoLoginState;

        private void Start()
        {
            _autoLoginState = GetSavedState();
            
            UpdateButton();
        }

        public void Toggle()
        {
            _autoLoginState = !_autoLoginState;
            
            Save();
            UpdateButton();
        }

        private bool GetSavedState()
        {
            return PlayerPrefs.GetInt(AUTO_LOGIN_KEY, 0) == 1;
        }

        private void UpdateButton()
        {
            Image.sprite = _autoLoginState ? ImageOn : ImageOff;
        }

        private void Save()
        {
            PlayerPrefs.SetInt(AUTO_LOGIN_KEY, _autoLoginState ? 1 : 0);
        }
    }
}