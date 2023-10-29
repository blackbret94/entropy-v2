/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Entropy.Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Serialization;
using Vashta.Entropy.SceneNavigation;
using Vashta.Entropy.UI;
using Vashta.Entropy.UI.TeamScore;

namespace TanksMP
{
    /// <summary>
    /// UI script for all elements, team events and user interactions in the game scene.
    /// </summary>
    public class UIGame : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Joystick components controlling player movement and actions on mobile devices.
        /// </summary>
        public UIJoystick[] controls;

        /// <summary>
        /// UI texts displaying kill scores for this local player.
        /// [0] = Kill Count, [1] = Death Count
        /// </summary>
        public Text[] killCounter;

        /// <summary>
        /// Mobile crosshair aiming indicator for local player.
        /// </summary>
        public GameObject aimIndicator;
        
        public DeathPanel DeathPanel;
        
        /// <summary>
        /// UI text for indicating game end and which team has won the round.
        /// </summary>
        public Text gameOverText;

        /// <summary>
        /// UI window gameobject activated on game end, offering sharing and restart buttons.
        /// </summary>
        public GameObject gameOverMenu;

        public FireButton fireButton;
        [FormerlySerializedAs("powerupIcon")] public PowerupIcon bulletIcon;
        
        public TeammateKilledPopup teammateKilledPopup;

        public PowerUpPanel PowerUpPanel;

        public SfxController SfxController;
        public MusicController MusicController;
        
        public SceneNavigator SceneNavigator;

        public DropCollectiblesButton DropCollectiblesButton;

        //initialize variables
        void Start()
        {
            //on non-mobile devices hide joystick controls, except in editor
            #if !UNITY_EDITOR && (UNITY_STANDALONE || UNITY_WEBGL)
                ToggleControls(false);
            #endif
            
            //on mobile devices enable additional aiming indicator
            #if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_WEBGL
            if (aimIndicator != null)
            {
                Transform indicator = Instantiate(aimIndicator).transform;
                indicator.SetParent(GameManager.GetInstance().localPlayer.shotPos);
                indicator.localPosition = new Vector3(0f, 0f, 3f);
            }
            #endif

            //play background music
            MusicController.PlayMusic();
        }


        /// <summary>
        /// This method gets called whenever room properties have been changed on the network.
        /// Updating our team size and score UI display during the game.
        /// See the official Photon docs for more details.
        /// </summary>
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
		{
			OnTeamSizeChanged(PhotonNetwork.CurrentRoom.GetSize());
			OnTeamScoreChanged(PhotonNetwork.CurrentRoom.GetScore());
		}


        /// <summary>
        /// This is an implementation for changes to the team fill,
        /// updating the slider values (updates UI display of team fill).
        /// </summary>
        public void OnTeamSizeChanged(int[] size)
        {
            TeamScoreController.GetInstance().UpdateTeamSizes(size);
        }
        
        /// <summary>
        /// This is an implementation for changes to the team score,
        /// updating the text values (updates UI display of team scores).
        /// </summary>
        public void OnTeamScoreChanged(int[] score)
        {
            TeamScoreController.GetInstance().UpdateScores(score);
        }
        
        /// <summary>
        /// Enables or disables visibility of joystick controls.
        /// </summary>
        public void ToggleControls(bool state)
        {
            for (int i = 0; i < controls.Length; i++)
                controls[i].gameObject.SetActive(state);
        }


        /// <summary>
        /// Sets death text showing who killed the player in its team color.
        /// Parameters: killer's name, killer's team
        /// </summary>
        public void SetDeathText(string playerName, Team team)
        {
            //hide joystick controls while displaying death text
            #if UNITY_EDITOR || (!UNITY_STANDALONE && !UNITY_WEBGL)
                ToggleControls(false);
                bulletIcon.SetActive(false);
            #endif
            
            DeathPanel.Set(playerName, team);
        }
        
        public void SetSpawnDelay(float time)
        {                
            DeathPanel.SetSpawnDelay(time);
        }
        
        
        /// <summary>
        /// Hides any UI components related to player death after respawn.
        /// </summary>
        public void DisableDeath()
        {
            //show joystick controls after disabling death text
            #if UNITY_EDITOR || (!UNITY_STANDALONE && !UNITY_WEBGL)
                ToggleControls(true);
            #endif
            
            //clear text component values
            DeathPanel.Clear();
        }


        /// <summary>
        /// Set game end text and display winning team in its team color.
        /// </summary>
        public void SetGameOverText(Team team)
        {
            //hide joystick controls while displaying game end text
            #if UNITY_EDITOR || (!UNITY_STANDALONE && !UNITY_WEBGL)
                ToggleControls(false);
                bulletIcon.SetActive(false);
            #endif
            
            //show winning team and colorize it by converting the team color to an HTML RGB hex value for UI markup
            gameOverText.text = "TEAM <color=#" + ColorUtility.ToHtmlStringRGB(team.material.color) + ">" + team.name + "</color> WINS!";
        }


        /// <summary>
        /// Displays the game's end screen. Called by GameManager after few seconds delay.
        /// Tries to display a video ad, if not shown already.
        /// </summary>
        public void ShowGameOver(int winningTeamIndex, string winningTeamName, Color winningTeamColor)
        {       
            //hide text but enable game over window
            gameOverText.gameObject.SetActive(false);
            MatchOverPanel.Get().Inflate(winningTeamName, winningTeamIndex, winningTeamColor);
            // gameOverMenu.SetActive(true);
            
            //check whether an ad was shown during the game
            //if no ad was shown during the whole round, we request one here
            #if UNITY_ADS
            if(!UnityAdsManager.didShowAd())
                UnityAdsManager.ShowAd(true);
            #endif
        }


        /// <summary>
        /// Returns to the starting scene and immediately requests another game session.
        /// In the starting scene we have the loading screen and disconnect handling set up already,
        /// so this saves us additional work of doing the same logic twice in the game scene. The
        /// restart request is implemented in another gameobject that lives throughout scene changes.
        /// </summary>
        public void Restart()
        {
            GameObject gObj = new GameObject("RestartNow");
            gObj.AddComponent<UIRestartButton>();
            DontDestroyOnLoad(gObj);
            
            Disconnect();
        }


        /// <summary>
        /// Stops receiving further network updates by hard disconnecting, then load starting scene.
        /// </summary>
        public void Disconnect()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }


        /// <summary>
        /// Loads the starting scene. Disconnecting already happened when presenting the GameOver screen.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneNavigator.GoToMainMenu();
        }
    }
}
