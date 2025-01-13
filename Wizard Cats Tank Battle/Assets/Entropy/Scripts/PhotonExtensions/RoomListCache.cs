using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomListCache : SimulationBehaviour, INetworkRunnerCallbacks
    {
        public UIMain UIMain { get; private set; }
        // private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

        private Dictionary<string, SessionInfo> cachedRoomList = new Dictionary<string, SessionInfo>();

        public Dictionary<string, SessionInfo> RoomList => cachedRoomList;
        
        public delegate void OnUpdatedCache();

        public static event OnUpdatedCache onUpdatedCache;

        private const float RefreshWaitTime = .5f;

        private void Start()
        {
            UIMain = UIMain.GetInstance();
            JoinLobby();
        }

        public void JoinLobby()
        {
            // TODO: Pass in sessionName, scene
            NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();
            runner.StartGame(new StartGameArgs { GameMode = Fusion.GameMode.Shared });
            //
            // Debug.Log("Attempted to join lobby: " + joinedLobby);

            StartCoroutine(RetryConnection());
            
        }

        public void RefreshLobbies()
        {
            // Clear text
            cachedRoomList.Clear();
            
            if (onUpdatedCache != null) onUpdatedCache();
            
            // Refresh
            JoinLobby();
        }

        private IEnumerator RetryConnection()
        {
            yield return new WaitForSeconds(RefreshWaitTime);
            JoinLobby();
        }

        private void UpdateCachedRoomList(List<SessionInfo> roomList)
        {
            for(int i=0; i<roomList.Count; i++)
            {
                SessionInfo info = roomList[i];
                if (!info.IsOpen || !info.IsVisible || !info.IsValid)
                {
                    cachedRoomList.Remove(info.Name);
                }
                else
                {
                    cachedRoomList[info.Name] = info;
                }
            }

            if (onUpdatedCache != null) onUpdatedCache();
        }
        
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
        { 
            cachedRoomList.Clear();
        }
        
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("Getting list of lobbies!");
            UpdateCachedRoomList(sessionList);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}