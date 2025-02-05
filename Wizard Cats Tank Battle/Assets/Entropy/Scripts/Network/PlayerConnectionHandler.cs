using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.GameState;
using Vashta.Entropy.Player;
using Vashta.Entropy.SaveLoad;

namespace Vashta.Entropy.Network
{
    public class PlayerConnectionHandler : SimulationBehaviour, IPlayerJoined, IPlayerLeft
    {
        public GameObject PlayerPrefab;
        private GameManager _gameManager;
        private NetworkManagerCustom _networkManagerCustom;

        private void Awake()
        {
            _networkManagerCustom = GetComponent<NetworkManagerCustom>();
        }
        
        private void Start()
        {
            _gameManager = GameManager.GetInstance();
        }
        
        public void PlayerJoined(PlayerRef playerRef)
        {
            // // Spawn player if local player
            // // if (Runner.LocalPlayer == playerRef)
            // // {
            //     // Choose team for player
            //     // int teamIndex = _gameManager.TeamController.GetTeamFill();
            //     
            //     // Spawn TODO: Improve so x and y are random
            //     // Transform spawnTransform = _gameManager.TeamController.teams[teamIndex].spawnArea;
            //     // Vector3 startPos = spawnTransform != null ? spawnTransform.position : Vector3.zero;
            //     // NetworkObject playerNetworkObject = Runner.Spawn(PlayerPrefab, startPos, Quaternion.identity);
            //     
            //     // Link player to network object
            //     // Runner.SetPlayerObject(playerRef, playerNetworkObject);
            //     
            //     // Player player = playerNetworkObject.GetComponent<Player>();
            //     Player player = _networkManagerCustom.GetPlayerGameObject(playerRef);
            //     
            //     if (player != null)
            //     {
            //         if (player.HasInputAuthority)
            //         {
            //             // This is probably deprecated
            //             // CharacterAppearanceSaveLoad.SetCurrentAppearanceAsCustomProperty();
            //         }
            //         
            //         // player.TeamIndex = teamIndex;
            //         // player.ApplyTeamChange();
            //     }
            //     else
            //     {
            //         Debug.LogError("Player " + playerRef + " is not a player");
            //     }
            // // }
        }
        
        public void PlayerLeft(PlayerRef player)
        {
            //get player-controlled game object from disconnected player
            PlayerController targetPlayerController = _networkManagerCustom.GetPlayerGameObject(player);

            //process any collectibles assigned to that player
            if(targetPlayerController != null)
            {
                Collectible[] collectibles = targetPlayerController.GetComponentsInChildren<Collectible>(true);
                for (int i = 0; i < collectibles.Length; i++)
                {
                    //let the player drop the Collectible
                    targetPlayerController.DropCollectibles();
                }
            }

            _gameManager.TeamController.RemovePlayerFromTeam(targetPlayerController);
            
            if (!Runner.IsShutdown)
            {
                Runner.Despawn(targetPlayerController.Object);
            }
            
            // remove player from Scoreboard
            // PhotonNetwork.CurrentRoom.AddOfflinePlayerToScoreboard(player);
        }
    }
}