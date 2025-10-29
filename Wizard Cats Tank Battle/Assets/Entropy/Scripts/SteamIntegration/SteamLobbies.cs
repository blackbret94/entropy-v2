using System.Collections;
using Fusion;
using Steamworks;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.PhotonExtensions;

namespace Vashta.Entropy.Scripts.SteamIntegration
{
    public class SteamLobbies : MonoBehaviour
    {
        protected Callback<GameRichPresenceJoinRequested_t> _joinRequested;

        private void Awake()
        {
            _joinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(OnJoinRequested);
        }
        
        // Broadcast the player's room to their steam profile
        public void SetJoinableRoom(RoomInfoWrapper roomInfoWrapper)
        {
            if (!SteamAPI.IsSteamRunning() || !SteamUser.GetSteamID().IsValid())
            {
                Debug.LogError("Invalid Steam connection, could not broadcast room info");
                return;
            }

            if (roomInfoWrapper == null)
            {
                SteamFriends.ClearRichPresence();
                return;
            }

            // Store a join string (e.g., room name or custom code)
            string joinCode = roomInfoWrapper.GetSessionInfo().Name; // or a custom field
            SteamFriends.SetRichPresence("connect", joinCode);

            string map = roomInfoWrapper.GetMapName();
            SteamFriends.SetRichPresence("status", map + " - In A Match");
        }

        public void ClearRoom()
        {
            if (!SteamAPI.IsSteamRunning() || !SteamUser.GetSteamID().IsValid())
            {
                Debug.LogError("Invalid Steam connection, could not clear room info");
                return;
            }

            SteamFriends.ClearRichPresence();
        }
        
        private void OnJoinRequested(GameRichPresenceJoinRequested_t callback)
        {
            string roomCode = callback.m_rgchConnect;
            Debug.Log($"Steam Join Requested: {roomCode}");

            // Connect to Photon Fusion using the join code
            NetworkManagerCustom networkManagerCustom = NetworkManagerCustom.GetInstance();

            if (networkManagerCustom != null)
            {
                networkManagerCustom.JoinRoom(roomCode);
            }
        }
    }
}