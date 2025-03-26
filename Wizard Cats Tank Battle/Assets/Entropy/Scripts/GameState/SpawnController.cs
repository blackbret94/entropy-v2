using System.Collections;
using Mono.Cecil.Cil;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.ClassSelectionPanel;
using Vashta.Entropy.Util;

namespace Vashta.Entropy.GameState
{
    public class SpawnController : MonoBehaviour
    {
        private GameManager _gameManager;
        
        /// <summary>
        /// The delay in seconds before respawning a player after it got killed.
        /// </summary>
        public int respawnTime = 5;

        private void Awake()
        {
            _gameManager = GetComponent<GameManager>();
        }
        
        /// <summary>
        /// Only for this player: sets the death text stating the killer on death.
        /// </summary>
        public void DisplayDeath(PlayerController playerToRespawn)
        {
            if (playerToRespawn == null)
            {
                Debug.LogError("Tried to respawn a null player");
                return;
            }
            
            if (!ClassSelectionPanel.Instance.CountdownIsActive())
            {
                //get the player component that killed us
                PlayerController other = playerToRespawn;
                string killedByName = "YOURSELF";
                if (playerToRespawn.killedBy != null)
                    other = playerToRespawn.killedBy.GetComponent<PlayerController>();

                //suicide or regular kill?
                if (other != playerToRespawn)
                {
                    killedByName = other.PlayerName;
                }

                //and start waiting for the respawn delay immediately
                _gameManager.ui.SetDeathText(killedByName, _gameManager.TeamController.teams[other.TeamIndex]);
            }

            StartSpawnRoutine(playerToRespawn);
        }

        public void StartSpawnRoutine(PlayerController playerToRespawn)
        {
            StartCoroutine(SpawnRoutine(playerToRespawn));
        }
        
        //coroutine spawning the player after a respawn delay
        // This is run on the local player's game
        private IEnumerator SpawnRoutine(PlayerController playerToRespawn)
        {
            if (playerToRespawn == null)
            {
                Debug.LogError("Tried to respawn a null player");
            }
            else
            {
                bool isLocalPlayer = playerToRespawn.HasInputAuthority;

                //calculate point in time for respawn
                Timer timer = new Timer(respawnTime, false);

                //wait for the respawn to be over,
                while (!timer.Run())
                {
                    float timeToSpawn = timer.GetTimeToRun();
                    
                    if(isLocalPlayer)
                        _gameManager.ui.SetSpawnDelay(timeToSpawn);
                    
                    yield return null;
                }

                if (isLocalPlayer)
                {
                    _gameManager.ui.DisableDeath();
                }

                if(playerToRespawn != null)
                    playerToRespawn.RPC_Respawn();
            }
        }
        
        public bool PlayerCanRespawnFreely(PlayerController player)
        {
            if (player.RespawnIsFreeFromJointime())
                return true;
            
            TeamDefinition playerTeamDefinition = player.GetTeamDefinition();

            // Check all potential team colliders
            foreach (var team in _gameManager.TeamController.teams)
            {
                if (playerTeamDefinition && team.teamDefinition.TeamId == playerTeamDefinition.TeamId)
                {
                    Collider col = team.freeClassChangeArea.GetComponent<Collider>();

                    if (col == null)
                    {
                        Debug.LogError("Team is missing a free respawn collider! " + player.TeamIndex);
                    }
                    else
                    {
                        if (col.bounds.Contains(player.transform.position))
                            return true;
                    }
                }
            }
            
            return false;
        }
    }
}