using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using FusionHelpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vashta.Entropy.GameState;
using Vashta.Entropy.Network;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.Scripts.CBSIntegration;
using Vashta.Entropy.UI.MapSelection;

namespace TanksMP
{
    /// <summary>
    /// Custom implementation of the most Photon callback handlers for network workflows. This script is
    /// responsible for connecting to Photon's Cloud, spawning players and handling disconnects.
    /// </summary>
    [RequireComponent(typeof(RoomOptionsFactory))]
    [RequireComponent(typeof(PlayerConnectionHandler))]
	public class NetworkManagerCustom : NetworkBehaviour, IPlayerJoined, INetworkRunnerCallbacks
    {
        //reference to this script instance
        private static NetworkManagerCustom instance;
        
        public RegionController RegionController { get; private set; }
        public UIMain UIMain { get; private set; }
        public LocalPlayerInfo LocalPlayerInfo;
        public WCTBSession WCTBSessionPrefab;
        private INetworkSceneManager _networkSceneManager;

        /// <summary>
        /// Scene index that gets loaded when disconnecting from a game.
        /// </summary>
        public string offlineSceneIndex = "MainMenu";

        /// <summary>
        /// Scene index that gets loaded after a connection has been established.
        /// Will get overridden by random matching scene, when using GameMode filtering.
        /// </summary>
        public string onlineSceneIndex = "Null";

        /// <summary>
        /// Maximum amount of players per room.
        /// </summary>
        public int maxPlayers = 12;

        /// <summary>
        /// Event fired when a connection to the matchmaker service failed.
        /// </summary>
        public static event Action connectionFailedEvent;
        
        private RoomOptionsFactory _roomOptionsFactory;

        public MapDefinitionDictionary MapDefinitionDictionary;
        private PlayerConnectionHandler _playerConnectionHandler;
        private FusionLauncher.ConnectionStatus _status = FusionLauncher.ConnectionStatus.Disconnected;

        //initialize network view
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            //adding a view to this gameobject with a unique viewID
            //this is to avoid having the same ID in a scene
            // PhotonView view = gameObject.AddComponent<PhotonView>();
            // view.ViewID = 999;
            
            // Get components
            _roomOptionsFactory = GetComponent<RoomOptionsFactory>();
            _playerConnectionHandler = GetComponent<PlayerConnectionHandler>();
            _networkSceneManager = GetComponent<INetworkSceneManager>(); 
            RegionController = new RegionController();

            if (_roomOptionsFactory == null)
            {
                Debug.LogError("Missing room options factory!");
            }
        }

        private void Start()
        {
            UIMain = UIMain.GetInstance();
        }
        
        public void TempNetworkStart()
        {
            LocalPlayerInfo.Name = CBSIntegrator.Instance.ProfileState.CachedDisplayName;
            FusionLauncher.Launch(Fusion.GameMode.Shared, "us", "WCTB", WCTBSessionPrefab, _networkSceneManager, OnConnectionStatusUpdate);
        }
        
        private void OnConnectionStatusUpdate(NetworkRunner runner, FusionLauncher.ConnectionStatus status, string reason)
        {
            if (!this)
                return;

            Debug.Log(status);

            if (status != _status)
            {
                switch (status)
                {
                    case FusionLauncher.ConnectionStatus.Disconnected:
                        Debug.LogError("Disconnected!");
                        break;
                    case FusionLauncher.ConnectionStatus.Failed:
                        Debug.LogError("Error");
                        break;
                }
            }

            _status = status;
        }
        
        /// <summary>
        /// Returns a reference to this script instance.
        /// </summary>
        public static NetworkManagerCustom GetInstance()
        {
            return instance;
        }
        
        /// <summary>
        /// Starts initializing and connecting to a game. Depends on the selected network mode.
        /// Sets the current player name prior to connecting to the servers.
        /// </summary>
        public void Connect(NetworkMode mode)
        {
            // PhotonNetwork.AutomaticallySyncScene = true;
            LocalPlayerInfo.Name = CBSIntegrator.Instance.ProfileState.CachedDisplayName;

            switch (mode)
            {
                //connects to a cloud game available on the Photon servers
                case NetworkMode.Online:
                    Runner.StartGame(new StartGameArgs { GameMode = Fusion.GameMode.Shared });
                    // TODO: Pass in sessionName, scene
                    break;

                //enable Photon offline mode to not send any network messages at all
                case NetworkMode.Offline:
                    StartCoroutine(Disconnect());
                    break;
            }
        }

        public void DisconnectFromServer()
        {
            StartCoroutine(Disconnect());
        }
        
        /// <summary>
        /// Need to disconnect before starting offline mode
        /// </summary>
        /// <returns></returns>
        private IEnumerator Disconnect()
        {
            Runner.Shutdown();
            
            while (Runner.IsRunning)
            {
                yield return null;
            }
            // PhotonNetwork.OfflineMode = true;
        }

        public void Reconnect()
        {
            StartCoroutine(ReconnectCoroutine());
        }

        private IEnumerator ReconnectCoroutine()
        {
            yield return StartCoroutine(Disconnect());
            
            PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);
            Connect(NetworkMode.Online);
        }

        public void CreateMatch(StartGameArgs startGameArgs)
        {
            Runner.StartGame(startGameArgs);
        }

        /// <summary>
        /// Joins a random room, will eventually take parameters.  This is for "Quickplay"
        /// </summary>
        public void JoinRandomRoom()
        {
            StartGameArgs startGameArgs = _roomOptionsFactory.CreateRoomOptionsGameMode((byte)PlayerPrefs.GetInt(PrefsKeys.gameMode));
            Runner.StartGame(startGameArgs);
        }

        public void JoinRandomRoom(string mapName, int gameMode)
        {
            StartGameArgs startGameArgs = _roomOptionsFactory.CreateRoomOptions(mapName, (byte)gameMode);
            Runner.StartGame(startGameArgs);
        }

        public void JoinRandomRoomOffline(StartGameArgs startGameArgs)
        {
            StartCoroutine(DisconnectAndJoinRoom(startGameArgs));
        }

        private IEnumerator DisconnectAndJoinRoom(StartGameArgs startGameArgs)
        {
            yield return StartCoroutine(Disconnect());
            startGameArgs.GameMode = Fusion.GameMode.Single;
            Runner.StartGame(startGameArgs);
        }

        /// <summary>
        /// Join a specific room by name
        /// </summary>
        public void JoinRoom(string roomName)
        {
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.SessionName = roomName;
            Runner.StartGame(startGameArgs);
        }

        /// <summary>
        /// Called when a creating a room failed. 
        /// See the official Photon docs for more details.
        /// </summary>
        // public override void OnCreateRoomFailed(short returnCode, string message)
        // {
        //     Debug.LogError("Error creating room: " + returnCode + " : " + message);
        //     
        //     if (connectionFailedEvent != null)
        //         connectionFailedEvent();
        // }


        /// <summary>
        /// Called when this client created a room and entered it.
        /// See the official Photon docs for more details.
        /// </summary>
        // public void OnSessionCreate(SessionInfo sessionInfo)
        // {
        //     string mapId = PlayerPrefs.GetString(PrefsKeys.selectedMap, "-1");
        //     MapDefinition mapDefinition = MapDefinitionDictionary[mapId];
        //     
        //     //the initial team size of the game for the server creating a new room.
        //     //unfortunately this cannot be set via the GameManager because it does not exist at that point
        //     short initialArrayLength = mapDefinition.TeamCount;
        //
        //     //we created a room so we have to set the initial room properties for this room,
        //     //such as populating the team fill and score arrays
        //     Hashtable roomProps = new Hashtable();
        //     roomProps.Add(RoomExtensions.size, new int[initialArrayLength]);
        //     roomProps.Add(RoomExtensions.score, new int[initialArrayLength]);
        //     PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        //
        //     // Load scene
        //     onlineSceneIndex = mapDefinition.SceneName;
        //     PhotonNetwork.LoadLevelFromBundle(onlineSceneIndex);
        // }
        
        //this wait routine is needed on offline mode for waiting on completed scene change,
        //because in offline mode Photon does not pause network messages. But it doesn't hurt
        //leaving this in for all other network modes too
        IEnumerator WaitForSceneChange()
        {
            while (SceneManager.GetActiveScene().name != onlineSceneIndex)
            {
                yield return null;
            }

            //we connected ourselves
            OnPlayerJoined(Runner, Runner.LocalPlayer);
        }

        /// <summary>
        /// Finds the remotely controlled Player game object of a specific player,
        /// by iterating over all Player components and searching for the matching creator.
        /// </summary>
        public Player GetPlayerGameObject(PlayerRef playerRef)
        {
            if (Runner.TryGetPlayerObject(playerRef, out NetworkObject playerObject))
            {
                Player player = playerObject.GetComponent<Player>();

                if (player == null)
                {
                    Debug.LogError("PlayerRef: " + playerRef.PlayerId + " does not contain Player component!");
                    return null;
                }

                return player;

            }
            else
            {
                Debug.LogError("Could not find PlayerRef: " + playerRef.PlayerId);
                return null;
            }
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            //we've joined a finished room, disconnect immediately
            if (GameManager.GetInstance() != null && GameManager.GetInstance().IsGameOver())
            {
                Runner.Shutdown();
                return;
            }

            //add ourselves to the game. This is only called for the master client
            //because other clients will trigger the OnPhotonPlayerConnected callback directly
            StartCoroutine(WaitForSceneChange());
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            UIGame uiGame = UIGame.GetInstance();
            if (uiGame != null)
            {
                uiGame.SceneNavigator.GoToMainMenu();
            }
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            if (reason != NetDisconnectReason.Requested &&
                connectionFailedEvent != null)
            {
                connectionFailedEvent();
            }

            // Debug.LogError("Disconnect cause: " + cause);

            //do not switch scenes automatically when the game over screen is being shown already
            if (GameManager.GetInstance() != null && GameManager.GetInstance().ui.gameOverMenu.activeInHierarchy)
                return;

            //switch from the online to the offline scene after connection is closed
            UIGame uiGame = UIGame.GetInstance();
            
            if (!uiGame.SceneNavigator.IsMainMenu())
            {
                uiGame.SceneNavigator.GoToMainMenu();
            }
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            throw new NotImplementedException();
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            // TODO: This is dated, a room should be created automatically if all else failed
            Debug.Log("Photon did not find any matches on the Master Client we are connected to. Creating our own room...");

            //joining failed so try to create our own room
            string mapId = PlayerPrefs.GetString(PrefsKeys.selectedMap, "-1");
            MapDefinition mapDefinition = MapDefinitionDictionary[mapId];

            string roomName = _roomOptionsFactory.CreateRoomNameFromPlayerNickname(LocalPlayerInfo.Name);
            byte maxPlayersForMap = (byte)mapDefinition.PlayerCount;
            string mapName = mapDefinition.Title;
            GameMode gameMode = (GameMode)PlayerPrefs.GetInt(PrefsKeys.gameMode, (int)GameMode.TDM);

            StartGameArgs startGameArgs = _roomOptionsFactory.CreateRoomOptions(roomName, mapName, maxPlayersForMap, gameMode);
            Runner.StartGame(startGameArgs);
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            throw new NotImplementedException();
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            throw new NotImplementedException();
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            LocalPlayerInfo.Name = CBSIntegrator.Instance.ProfileState.CachedDisplayName;
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            throw new NotImplementedException();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            throw new NotImplementedException();
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            throw new NotImplementedException();
        }

        public void PlayerJoined(PlayerRef player)
        {
            throw new NotImplementedException();
        }
    }
}