/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Entropy.Scripts.Audio;
using Entropy.Scripts.Player;
using Fusion;
using UnityEngine;
using Vashta.Entropy.GameMode;
using Vashta.Entropy.GameState;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

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
        public PlayerController localPlayerController;
        
        [Header("Controllers")]
        public UIGame ui;
        public BotController BotController { get; private set; }
        public MusicController MusicController;
        public PlayerInputController PlayerInputController;
        public SfxController SfxController;
        public GameObject InitialSpawnPos;
        public TeamController TeamController { get; private set; }
        public GameOverController GameOverController { get; private set; }
        public SpawnController SpawnController { get; private set; }
        public MatchTimer MatchTimer { get; private set; }
        
        [SerializeField]
        private MapDefinition mapDefinition;
        
        [Header("Data Sources")]
        public GameModeDictionary GameModeDictionary;

        public GameModeDefinition GameModeDefinition => GameModeDictionary[MatchInfo.GameMode];

        private NetworkManagerCustom _networkManager;
        
        [Networked]
        public MatchInfo MatchInfo { get; private set; }
        
        [Networked]
        public bool GameHasEnded { get; private set; }
        public bool HasSpawned { get; private set; }

        //initialize variables
        void Awake()
        {
            instance = this;
            _networkManager = NetworkManagerCustom.GetInstance();
            
            BotController = GetComponent<BotController>();
            TeamController = GetComponent<TeamController>();
            GameOverController = GetComponent<GameOverController>();
            SpawnController = GetComponent<SpawnController>();
            MatchTimer = GetComponent<MatchTimer>();
        }

        private void CreateMatchInfo()
        {
            if (!HasStateAuthority)
                return;

            MatchInfo matchInfo = new MatchInfo();
            LocalPlayerInfo localPlayerInfo = _networkManager.LocalPlayerInfo;
            

            SessionInfo sessionInfo = Runner.SessionInfo;
            RoomInfoWrapper roomInfoWrapper = new RoomInfoWrapper(sessionInfo);
            
            matchInfo.GameMode = localPlayerInfo.GameModeEnum;
            matchInfo.MaxNumberOfPlayers = roomInfoWrapper.GetMaxPlayers();
            matchInfo.BotFilling = roomInfoWrapper.BotFilling();
            matchInfo.MapName = roomInfoWrapper.GetMapName();
            matchInfo.MaxScoreToWin = GameModeDefinition.GetScoreToWin();
            
            MatchInfo = matchInfo;
            
            TeamController.maxScore = matchInfo.MaxScoreToWin;
        }

        public override void Spawned()
        {
            base.Spawned();
            
            if (HasStateAuthority)
            {
                CreateMatchInfo();

                if (MatchInfo.BotFilling)
                {
                    BotController.maxBots = MatchInfo.MaxNumberOfPlayers;
                    BotController.maxPlayers = MatchInfo.MaxNumberOfPlayers;
                    StartCoroutine(BotController.InitialSpawnCR());
                }
            }
            
            HasSpawned = true;
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
            if (GameHasEnded)
                return true;
            
            if (!MatchTimer.MatchTimeIsRunning())
            {
                GameHasEnded = true;
                return true;
            }

            GameHasEnded = TeamController.MaxScoreIsReached();
            return GameHasEnded;
        }

        public void MarkGameOver()
        {
            GameHasEnded = true;
        }
    }
}