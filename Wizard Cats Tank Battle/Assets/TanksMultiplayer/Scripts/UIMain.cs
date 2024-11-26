/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System;
using System.Collections;
using Entropy.Scripts.Audio;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.Character;
using Vashta.Entropy.IO;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.SaveLoad;
using Vashta.Entropy.SceneNavigation;
using Vashta.Entropy.TanksExtensions;
using Vashta.Entropy.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TanksMP
{
    /// <summary>
    /// UI script for all elements, settings and user interactions in the menu scene.
    /// </summary>
    public class UIMain : MonoBehaviour
    {
        public CatNameGenerator CatNameGenerator;
        
        /// <summary>
        /// Window object for loading screen between connecting and scene switch.
        /// </summary>
        public GameObject loadingWindow;

        /// <summary>
        /// Window object for displaying errors with the connection or timeouts.
        /// </summary>
        public GameObject connectionErrorWindow;

        /// <summary>
        /// Window object for displaying errors with the billing actions.
        /// </summary>
        public GameObject billingErrorWindow;

        public IntroductionPanel IntroductionPanel;
        public MusicController MusicController;
        public SceneNavigator SceneNavigator;

        public string WebsiteUrl = "https://wizardcatstankbattle.com";
        public string PrivacyPolicyUrl = "https://vashtaentertainment.com/privacy_policy.html";

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

            //read the selections and set them in the corresponding UI elements

            // read music and volume levels
            if(SettingsReader.GetMusicIsOn())
                MusicController.PlayMusic();
            
            AudioListener.volume = SettingsReader.GetVolume();
            
            // read graphics quality
            QualitySettings.SetQualityLevel(SettingsReader.GetGraphicsSettings());

            //listen to network connection and IAP billing errors
            NetworkManagerCustom.connectionFailedEvent += OnConnectionError;
            UnityIAPManager.purchaseFailedEvent += OnBillingError;
            
            IntroductionPanel.Init();
        }
        
        /// <summary>
        /// Tries to enter the game scene. Sets the loading screen active while connecting to the
        /// Matchmaker and starts the timeout coroutine at the same time.
        /// </summary>
        public void Play()
        {
            NetworkMode networkMode = (NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode);
            
            loadingWindow.SetActive(true);

            if (networkMode == NetworkMode.Online)
            {
                // Join online
                NetworkManagerCustom.JoinRandomRoom();
            }
            else
            {
                // Join offline
                NetworkManagerCustom.GetInstance().JoinRandomRoomOffline(new Hashtable());
            }
            
            // NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            StartCoroutine(HandleTimeout());
        }

        public void Play(string mapName, int gameMode)
        {
            loadingWindow.SetActive(true);
            
            Hashtable expectedCustomRoomProperties = 
                new Hashtable()
                {
                    { RoomKeys.mapKey, mapName},
                    { RoomKeys.modeKey, (byte)gameMode }
                };
            
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
            StartCoroutine(HandleTimeout());
        }

        public void PlayOffline(string mapName, int gameMode)
        {
            loadingWindow.SetActive(true);
            
            Hashtable expectedCustomRoomProperties = 
                new Hashtable()
                {
                    { RoomKeys.mapKey, mapName},
                    { RoomKeys.modeKey, (byte)gameMode }
                };
            
            NetworkManagerCustom.GetInstance().JoinRandomRoomOffline(expectedCustomRoomProperties);
        }

        public void JoinRoom(string roomName)
        {
            loadingWindow.SetActive(true);
            NetworkManagerCustom.JoinRoom(roomName);
            // NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            StartCoroutine(HandleTimeout());
        }

        public void CreateRoom(RoomOptions roomOptions)
        {
            loadingWindow.SetActive(true);
            NetworkManagerCustom.CreateMatch(roomOptions);
            StartCoroutine(HandleTimeout());
        }
        
        //coroutine that waits 10 seconds before cancelling joining a match
        IEnumerator HandleTimeout()
        {
            yield return new WaitForSeconds(10);

            //timeout has passed, we would like to stop joining a game now
            PhotonNetwork.Disconnect();
            //display connection issue window
            OnConnectionError();
        }


        //activates the connection error window to be visible
        void OnConnectionError()
        {
            //game shut down completely
            if (this == null)
                return;

            Debug.LogError("Connection error");
            
            StopAllCoroutines();
            loadingWindow.SetActive(false);
            connectionErrorWindow.SetActive(true);
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
        
        //activates the billing error window to be visible
        void OnBillingError(string error)
        {
            //get text label to display billing failed reason
            Text errorLabel = billingErrorWindow.GetComponentInChildren<Text>();
            if (errorLabel)
                errorLabel.text = "Purchase failed.\n" + error;

            billingErrorWindow.SetActive(true);
        }
		
        /// <summary>
        /// Opens a browser window to the App Store entry for this app.
        /// </summary>
        public void RateApp()
        {
            //UnityAnalyticsManager.RateStart();
            
            //default app url on non-mobile platforms
            //replace with your website, for example
			string url = "";
			
			#if UNITY_ANDROID
				url = "http://play.google.com/store/apps/details?id=" + Application.identifier;
			#elif UNITY_IPHONE
				url = "https://itunes.apple.com/app/idXXXXXXXXX";
			#endif
			
			if(string.IsNullOrEmpty(url) || url.EndsWith("XXXXXX"))
            {
                Debug.LogWarning("UIMain: You didn't replace your app links!");
                return;
            }
			
			Application.OpenURL(url);
        }

        public void OpenWebsite()
        {
            Application.OpenURL(WebsiteUrl);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(PrivacyPolicyUrl);
        }
    }
}