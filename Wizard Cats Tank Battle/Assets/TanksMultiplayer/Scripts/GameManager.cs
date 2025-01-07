/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Entropy.Scripts.Audio;
using Entropy.Scripts.Player;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.GameMode;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.GameState;
using Vashta.Entropy.Scoreboard;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace TanksMP
{
    /// <summary>
    /// Manages game workflow and provides high-level access to networked logic during a game.
    /// It manages functions such as team fill, scores and ending a game, but also video ad results.
    /// </summary>
    [RequireComponent(typeof(MatchTimer))]
    [RequireComponent(typeof(BotController))]
    [RequireComponent(typeof(TeamController))]
    [RequireComponent(typeof(GameOverController))]
    [RequireComponent(typeof(SpawnController))]
    public class GameManager : NetworkBehaviour
    {
        //reference to this script instance
        private static GameManager instance;

        /// <summary>
        /// The local player instance spawned for this client.
        /// </summary>
        [HideInInspector]
        public Player localPlayer;
        public GameMode gameMode = GameMode.TDM;
        
        [Header("Controllers")]
        public UIGame ui;
        public BotController BotController { get; private set; }
        public MusicController MusicController;
        public PlayerInputController PlayerInputController;
        public SfxController SfxController;
        public TeamController TeamController { get; private set; }
        public GameOverController GameOverController { get; private set; }
        public SpawnController SpawnController { get; private set; }
        public MatchTimer MatchTimer { get; private set; }
        
        [FormerlySerializedAs("_mapDefinition")] [SerializeField]
        private MapDefinition mapDefinition;
        
        [Header("Data Sources")]
        public GameModeDictionary GameModeDictionary;
        
        public GameModeDefinition GameModeDefinition { get; private set; }

        private NetworkManagerCustom _networkManager;

        //initialize variables
        void Awake()
        {
            instance = this;
            _networkManager = NetworkManagerCustom.GetInstance();

            gameMode = _networkManager.LocalPlayerInfo.GameModeEnum;
            GameModeDefinition = GameModeDictionary[gameMode];
            
            BotController = GetComponent<BotController>();
            TeamController = GetComponent<TeamController>();
            GameOverController = GetComponent<GameOverController>();
            SpawnController = GetComponent<SpawnController>();
            MatchTimer = GetComponent<MatchTimer>();

            TeamController.maxScore = GameModeDefinition.ScoreToWin;

            MatchTimer.InitTimer();            

            //if Unity Ads is enabled, hook up its result callback
#if UNITY_ADS
                UnityAdsManager.adResultEvent += HandleAdResult;
#endif
        }
        
        /// <summary>
        /// Returns a reference to this script instance.
        /// </summary>
        public static GameManager GetInstance()
        {
            return instance;
        }

        public bool IsGameOver()
        {
            if (!MatchTimer.MatchTimeIsRunning())
                return true;

            return TeamController.MaxScoreIsReached();
        }
        
        //implements what to do when an ad view completes
        #if UNITY_ADS
        void HandleAdResult(ShowResult result)
        {
            switch (result)
            {
                //in case the player successfully watched an ad,
                //it sends a request for it be respawned
                case ShowResult.Finished:
                case ShowResult.Skipped:
                    localPlayer.CmdRespawn();
                    break;
                
                //in case the ad can't be shown, just handle it
                //like we wouldn't have tried showing a video ad
                //with the regular death countdown (force ad skip)
                case ShowResult.Failed:
                    DisplayDeath(true);
                    break;
            }
        }
        #endif
        
        //clean up callbacks on scene switches
        void OnDestroy()
        {
            #if UNITY_ADS
                UnityAdsManager.adResultEvent -= HandleAdResult;
            #endif
        }
    }
}