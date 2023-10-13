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

        /// <summary>
        /// Settings: dropdown selection for network mode.
        /// </summary>
        public Dropdown networkDrop;

        /// <summary>
		/// Settings: input field for manual server address,
        /// hosting a server in a private network (Photon only).
		/// </summary>
		public InputField serverField;

        /// <summary>
        /// Settings: checkbox for playing background music.
        /// </summary>
        public Toggle musicToggle;
        
        public Toggle leftHandedModeToggle;

        /// <summary>
        /// Settings: slider for adjusting game sound volume.
        /// </summary>
        public Slider volumeSlider;

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
            // if (!PlayerPrefs.HasKey(PrefsKeys.playerName)) PlayerPrefs.SetString(PrefsKeys.playerName, CatNameGenerator.GetRandomName());
            if (!PlayerPrefs.HasKey(PrefsKeys.networkMode)) PlayerPrefs.SetInt(PrefsKeys.networkMode, 0);
            if (!PlayerPrefs.HasKey(PrefsKeys.gameMode)) PlayerPrefs.SetInt(PrefsKeys.gameMode, 0);
            if (!PlayerPrefs.HasKey(PrefsKeys.serverAddress)) PlayerPrefs.SetString(PrefsKeys.serverAddress, "127.0.0.1");
            if (!PlayerPrefs.HasKey(PrefsKeys.playMusic)) PlayerPrefs.SetString(PrefsKeys.playMusic, "true");
            if (!PlayerPrefs.HasKey(PrefsKeys.appVolume)) PlayerPrefs.SetFloat(PrefsKeys.appVolume, 1f);
            if (!PlayerPrefs.HasKey(PrefsKeys.activeTank)) PlayerPrefs.SetString(PrefsKeys.activeTank, Encryptor.Encrypt("0"));
            if (!PlayerPrefs.HasKey(Vashta.Entropy.SaveLoad.PrefsKeys.characterAppearance)) PlayerPrefs.SetString(Vashta.Entropy.SaveLoad.PrefsKeys.characterAppearance, CharacterAppearanceSaveLoad.DefaultAppearanceStringEncrypted());
            if(!PlayerPrefs.HasKey(PrefsKeys.lefthandedMode)) PlayerPrefs.SetInt(PrefsKeys.lefthandedMode, 0);

            PlayerPrefs.Save();
            _playerNameVerification.VerifyName();

            //read the selections and set them in the corresponding UI elements
            networkDrop.value = PlayerPrefs.GetInt(PrefsKeys.networkMode);
            serverField.text = PlayerPrefs.GetString(PrefsKeys.serverAddress);
            musicToggle.isOn = bool.Parse(PlayerPrefs.GetString(PrefsKeys.playMusic));
            leftHandedModeToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.lefthandedMode, 0));
            volumeSlider.value = PlayerPrefs.GetFloat(PrefsKeys.appVolume);

            //call the onValueChanged callbacks once with their saved values
            OnMusicChanged(musicToggle.isOn);
            OnVolumeChanged(volumeSlider.value);

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
            loadingWindow.SetActive(true);
            NetworkManagerCustom.JoinRandomRoom();
            // NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            StartCoroutine(HandleTimeout());
        }

        public void Play(string mapName, int gameMode)
        {
            loadingWindow.SetActive(true);
            
            ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = 
                new ExitGames.Client.Photon.Hashtable()
                {
                    {"map", mapName},
                    { "mode", (byte)gameMode }
                };
            
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
            StartCoroutine(HandleTimeout());
        }

        public void PlayOffline(string mapName, int gameMode)
        {
            loadingWindow.SetActive(true);
            NetworkManagerCustom.JoinRandomRoom();
            StartCoroutine(HandleTimeout());
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
            Photon.Pun.PhotonNetwork.Disconnect();
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
        /// Allow additional input of server address only in network mode LAN.
        /// Otherwise, the input field will be hidden in the settings (Photon only).
        /// </summary>
        public void OnNetworkChanged(int value)
        {
            serverField.gameObject.SetActive((NetworkMode)value == NetworkMode.LAN ? true : false);
        }


        /// <summary>
        /// Save newly selected GameMode value to PlayerPrefs in order to check it later.
        /// Called by DropDown onValueChanged event.
        /// This should no longer be in PlayerPrefs
        /// </summary>
        public void OnGameModeChanged(int value)
        {
            // PlayerPrefs.SetInt(PrefsKeys.gameMode, value);
            // PlayerPrefs.Save();
        }


        /// <summary>
        /// Modify music AudioSource based on player selection.
        /// Called by Toggle onValueChanged event.
        /// </summary>
        public void OnMusicChanged(bool value)
        {
            MusicController.AudioSource.enabled = musicToggle.isOn;
            MusicController.PlayMusic();
        }


        /// <summary>
        /// Modify global game volume based on player selection.
        /// Called by Slider onValueChanged event.
        /// </summary>
        public void OnVolumeChanged(float value)
        {
            volumeSlider.value = value;
            AudioListener.volume = value;
        }


        /// <summary>
        /// Saves all player selections chosen in the Settings window on the device.
        /// </summary>
        public void CloseSettings()
        {
            PlayerPrefs.SetString(PrefsKeys.serverAddress, serverField.text);
            PlayerPrefs.SetString(PrefsKeys.playMusic, musicToggle.isOn.ToString());
            PlayerPrefs.SetFloat(PrefsKeys.appVolume, volumeSlider.value);
            PlayerPrefs.SetInt(PrefsKeys.lefthandedMode, leftHandedModeToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
            
            _playerNameVerification.VerifyName();
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