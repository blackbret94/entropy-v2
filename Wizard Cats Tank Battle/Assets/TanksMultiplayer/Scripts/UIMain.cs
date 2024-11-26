/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Entropy.Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.IO;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.SaveLoad;
using Vashta.Entropy.SceneNavigation;
using Vashta.Entropy.TanksExtensions;
using Vashta.Entropy.UI;

namespace TanksMP
{
    /// <summary>
    /// UI script for all elements, settings and user interactions in the menu scene.
    /// </summary>
    public class UIMain : MonoBehaviour
    {
        public CatNameGenerator CatNameGenerator;
        
        public GameObject loadingWindow;
        public GameObject connectionErrorWindow;

        public IntroductionPanel IntroductionPanel;
        public MusicController MusicController;
        public SceneNavigator SceneNavigator;
        public RoomController RoomController;

        private PlayerNameVerification _playerNameVerification;

        private static UIMain _instance;
        public static UIMain GetInstance() => _instance;
        
        //initialize player selection in Settings window
        //if this is the first time launching the game, set initial values
        void Start()
        {
            _instance = this;
            
            _playerNameVerification = new PlayerNameVerification(CatNameGenerator);
            
            //set initial values for all settings
            if (!PlayerPrefs.HasKey(PrefsKeys.networkMode)) PlayerPrefs.SetInt(PrefsKeys.networkMode, 0);
            if (!PlayerPrefs.HasKey(PrefsKeys.gameMode)) PlayerPrefs.SetInt(PrefsKeys.gameMode, 0);
            if (!PlayerPrefs.HasKey(PrefsKeys.serverAddress)) PlayerPrefs.SetString(PrefsKeys.serverAddress, "127.0.0.1");
            if (!PlayerPrefs.HasKey(PrefsKeys.playMusic)) PlayerPrefs.SetString(PrefsKeys.playMusic, "true");
            if (!PlayerPrefs.HasKey(PrefsKeys.appVolume)) PlayerPrefs.SetFloat(PrefsKeys.appVolume, 1f);
            if (!PlayerPrefs.HasKey(PrefsKeys.activeTank)) PlayerPrefs.SetString(PrefsKeys.activeTank, Encryptor.Encrypt("0"));
            if (!PlayerPrefs.HasKey(Vashta.Entropy.SaveLoad.PrefsKeys.characterAppearance)) PlayerPrefs.SetString(Vashta.Entropy.SaveLoad.PrefsKeys.characterAppearance, CharacterAppearanceSaveLoad.DefaultAppearanceStringEncrypted());
            if(!PlayerPrefs.HasKey(PrefsKeys.lefthandedMode)) PlayerPrefs.SetInt(PrefsKeys.lefthandedMode, 0);
            if(!PlayerPrefs.HasKey(PrefsKeys.aimArrow)) PlayerPrefs.SetInt(PrefsKeys.aimArrow, 0);

            PlayerPrefs.Save();
            _playerNameVerification.VerifyName();

            // read music and volume levels
            if(SettingsReader.GetMusicIsOn())
                MusicController.PlayMusic();
            
            AudioListener.volume = SettingsReader.GetVolume();
            
            // read graphics quality
            QualitySettings.SetQualityLevel(SettingsReader.GetGraphicsSettings());

            //listen to network connection and IAP billing errors
            NetworkManagerCustom.connectionFailedEvent += RoomController.OnConnectionError;
            
            IntroductionPanel.Init();
        }

        public void ShowConnectionErrorWindow()
        {
            loadingWindow.SetActive(false);
            connectionErrorWindow.SetActive(true);
        }

        public void ToggleLoadingWindow(bool b)
        {
            loadingWindow.SetActive(b);
        }

        public void ReturnToLoginScreen()
        {
            if (SceneNavigator == null)
            {
                Debug.LogError("Missing SceneNavigator link!");
                return;
            }
            
            SceneNavigator.GoToLogin();
        }
    }
}