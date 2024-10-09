using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Vashta.Entropy.PhotonExtensions
{
    public class RoomListCache : MonoBehaviourPunCallbacks
    {
        private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

        private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

        public Dictionary<string, RoomInfo> RoomList => cachedRoomList;
        
        public delegate void OnUpdatedCache();

        public static event OnUpdatedCache onUpdatedCache;

        private const float RefreshWaitTime = .5f;

        private void Start()
        {
            JoinLobby();
        }

        public void JoinLobby()
        {
            PhotonNetwork.ConnectUsingSettings();
            bool joinedLobby = PhotonNetwork.JoinLobby();
            //
            // Debug.Log("Attempted to join lobby: " + joinedLobby);

            if (!joinedLobby)
            {
                StartCoroutine(RetryConnection());
            }
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

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            for(int i=0; i<roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                if (info.RemovedFromList)
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

        public override void OnJoinedLobby()
        {
            cachedRoomList.Clear();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Getting list of lobbies!");
            UpdateCachedRoomList(roomList);
        }

        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            cachedRoomList.Clear();
        }
    }
}