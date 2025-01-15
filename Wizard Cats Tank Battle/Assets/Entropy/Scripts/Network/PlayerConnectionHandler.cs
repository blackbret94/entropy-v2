using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.GameState;
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
            // TODO: IS this causing the spam?
            return;
            // Spawn player if local player
            if (Runner.LocalPlayer == playerRef)
            {
                // Choose team for player
                int teamIndex = _gameManager.TeamController.GetTeamFill();
                
                // Spawn TODO: Improve so x and y are random
                Transform spawnTransform = _gameManager.TeamController.teams[teamIndex].spawnArea;
                Vector3 startPos = spawnTransform != null ? spawnTransform.position : Vector3.zero;
                NetworkObject playerNetworkObject = Runner.Spawn(PlayerPrefab, startPos, Quaternion.identity);
                
                // Link player to network object
                Runner.SetPlayerObject(playerRef, playerNetworkObject);
                
                Player player = playerNetworkObject.GetComponent<Player>();

                if (player != null)
                {
                    player.TeamIndex = teamIndex;
                    CharacterAppearanceSaveLoad.SetCurrentAppearanceAsCustomProperty();
                }
                else
                {
                    Debug.LogError("Player " + playerRef + " is not a player");
                }
            }
        }
        
        public void PlayerLeft(PlayerRef player)
        {
            //get player-controlled game object from disconnected player
            Player targetPlayer = _networkManagerCustom.GetPlayerGameObject(player);

            //process any collectibles assigned to that player
            if(targetPlayer != null)
            {
                Collectible[] collectibles = targetPlayer.GetComponentsInChildren<Collectible>(true);
                for (int i = 0; i < collectibles.Length; i++)
                {
                    //let the player drop the Collectible
                    targetPlayer.DropCollectibles();
                }
            }

            _gameManager.TeamController.RemovePlayerFromTeam(targetPlayer);
            
            if (!Runner.IsShutdown)
            {
                Runner.Despawn(targetPlayer.Object);
            }
            
            // remove player from Scoreboard
            // PhotonNetwork.CurrentRoom.AddOfflinePlayerToScoreboard(player);
        }
    }
}