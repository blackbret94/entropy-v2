using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using FusionHelpers;
using UnityEngine;
using Vashta.Entropy.GameState;
using Vashta.Entropy.Network;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.SceneNavigation;
using Vashta.Entropy.Scripts.CBSIntegration;
using Vashta.Entropy.Scripts.SteamIntegration;
using Vashta.Entropy.UI.MapSelection;

namespace TanksMP
{
    /// <summary>
    /// Custom implementation of the most Photon callback handlers for network workflows. This script is
    /// responsible for connecting to Photon's Cloud, spawning players and handling disconnects.
    /// </summary>
    [RequireComponent(typeof(RoomOptionsFactory))]
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
        /// Event fired when a connection to the matchmaker service failed.
        /// </summary>
        public static event Action connectionFailedEvent;
        
        private RoomOptionsFactory _roomOptionsFactory;

        public MapDefinitionDictionary MapDefinitionDictionary;
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
        
        private void OnConnectionStatusUpdate(NetworkRunner runner, FusionLauncher.ConnectionStatus status, string reason)
        {
            if (!this)
                return;

            // Debug.Log(status);

            // if (status != _status)
            // {
            //     switch (status)
            //     {
            //         case FusionLauncher.ConnectionStatus.Disconnected:
            //             Debug.Log("Disconnected!");
            //             break;
            //         case FusionLauncher.ConnectionStatus.Failed:
            //             Debug.LogError("Error");
            //             break;
            //     }
            // }

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
                    StartGameArgs startGameArgs = new StartGameArgs { GameMode = Fusion.GameMode.Shared };
                    FusionLauncher.Launch(startGameArgs, "us", WCTBSessionPrefab, _networkSceneManager, OnConnectionStatusUpdate);
                    // TODO: Pass in sessionName, scene
                    break;

                //enable Photon offline mode to not send any network messages at all
                case NetworkMode.Offline:
                    StartCoroutine(Disconnect());
                    break;
            }
        }
        
        /// <summary>
        /// Need to disconnect before starting offline mode
        /// </summary>
        /// <returns></returns>
        private IEnumerator Disconnect()
        {
            SteamManager steamManager = SteamManager.Instance;
            if (steamManager)
            {
                steamManager.SteamLobbies.ClearRoom();
            }
            
            if (Runner)
            {
                if (Runner.IsRunning)
                    Runner.Shutdown(false);

                while (Runner.IsRunning)
                {
                    yield return null;
                }
                // PhotonNetwork.OfflineMode = true;
            }
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
            LocalPlayerInfo.Name = CBSIntegrator.Instance.ProfileState.CachedDisplayName;

            if (startGameArgs.SessionProperties != null)
            {
                if (startGameArgs.SessionProperties.ContainsKey(RoomKeys.modeKey))
                {
                    LocalPlayerInfo.GameMode = startGameArgs.SessionProperties[RoomKeys.modeKey];
                }

                if (startGameArgs.SessionProperties.ContainsKey(RoomKeys.mapKey))
                {
                    LocalPlayerInfo.MapName = startGameArgs.SessionProperties[RoomKeys.mapKey];
                }
            }

            FusionLauncher.Launch(startGameArgs, "us", WCTBSessionPrefab, _networkSceneManager, OnConnectionStatusUpdate);
        }

        public void CreateRandomMatch(StartGameArgs startGameArgs)
        {
            LocalPlayerInfo.Name = CBSIntegrator.Instance.ProfileState.CachedDisplayName;

            FusionLauncher.LaunchRandom(startGameArgs, "us", WCTBSessionPrefab, _networkSceneManager, OnConnectionStatusUpdate);
        }

        /// <summary>
        /// Joins a random room, will eventually take parameters.  This is for "Quickplay"
        /// </summary>
        public void JoinRandomRoom()
        {
            LocalPlayerInfo.Name = CBSIntegrator.Instance.ProfileState.CachedDisplayName;
            StartGameArgs startGameArgs = _roomOptionsFactory.CreateRoomOptionsGameMode((byte)PlayerPrefs.GetInt(PrefsKeys.gameMode));
            FusionLauncher.Launch(startGameArgs, "us", WCTBSessionPrefab, _networkSceneManager, OnConnectionStatusUpdate);
        }

        public void JoinRandomRoomOffline(StartGameArgs startGameArgs)
        {
            StartCoroutine(DisconnectAndJoinRoom(startGameArgs));
        }

        private IEnumerator DisconnectAndJoinRoom(StartGameArgs startGameArgs)
        {
            yield return StartCoroutine(Disconnect());
            startGameArgs.GameMode = Fusion.GameMode.Single;
            FusionLauncher.Launch(startGameArgs, "us", WCTBSessionPrefab, _networkSceneManager, OnConnectionStatusUpdate);
        }

        /// <summary>
        /// Join a specific room by name
        /// </summary>
        public void JoinRoom(string roomName)
        {
            StartCoroutine(JoinRoomAsync(roomName));
        }

        private IEnumerator JoinRoomAsync(string roomName)
        {
            if (Runner && Runner.IsRunning)
                yield return Disconnect();
            
            LocalPlayerInfo.Name = CBSIntegrator.Instance.ProfileState.CachedDisplayName;
            FusionLauncher.LaunchByName(roomName, "us", WCTBSessionPrefab, _networkSceneManager, OnConnectionStatusUpdate);
        }
        
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            GameManager gameManager = GameManager.GetInstance();
            
            //we've joined a finished room, disconnect immediately
            if (gameManager != null && gameManager.IsGameOver())
            {
                Runner.Disconnect(player);
                return;
            }

            //add ourselves to the game. This is only called for the master client
            //because other clients will trigger the OnPhotonPlayerConnected callback directly
            // StartCoroutine(WaitForSceneChange());
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            SteamManager steamManager = SteamManager.Instance;
            if (steamManager)
            {
                steamManager.SteamLobbies.ClearRoom();
            }
            
            //do not switch scenes automatically when the game is already over
            if (GameManager.GetInstance().IsGameOver())
                return;
            
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

            Debug.LogError("Disconnect cause: " + reason);
            
            SteamManager steamManager = SteamManager.Instance;
            if (steamManager)
            {
                steamManager.SteamLobbies.ClearRoom();
            }

            //do not switch scenes automatically when the game is already over
            if (GameManager.GetInstance().IsGameOver())
                return;

            //switch from the online to the offline scene after connection is closed
            
            if (!SceneNavigator.IsMainMenu())
            {
                UIGame uiGame = UIGame.GetInstance();
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
            Debug.LogError("Failed to connect to room!");
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