using System.Collections;
using Mono.Cecil.Cil;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.ClassSelectionPanel;

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
        public void DisplayDeath()
        {
            if (!ClassSelectionPanel.Instance.CountdownIsActive())
            {
                PlayerController localPlayerController = _gameManager.localPlayerController;
                //get the player component that killed us
                PlayerController other = localPlayerController;
                string killedByName = "YOURSELF";
                if (localPlayerController.killedBy != null)
                    other = localPlayerController.killedBy.GetComponent<PlayerController>();

                //suicide or regular kill?
                if (other != localPlayerController)
                {
                    killedByName = other.PlayerName;
                }

                //and start waiting for the respawn delay immediately
                _gameManager.ui.SetDeathText(killedByName, _gameManager.TeamController.teams[other.TeamIndex]);
            }

            StartCoroutine(SpawnRoutine());
        }


        //coroutine spawning the player after a respawn delay
        // This is run on the local player's game
        public IEnumerator SpawnRoutine()
        {
            //calculate point in time for respawn
            float targetTime = 0f;
            if (!ClassSelectionPanel.Instance.CountdownIsActive())
            {
                targetTime = Time.time + respawnTime;
            }
            
            //wait for the respawn to be over,
            //while waiting update the respawn countdown
            while (targetTime - Time.time > 0)
            {
                float timeToSpawn = targetTime - Time.time;
                _gameManager.ui.SetSpawnDelay(timeToSpawn);
                yield return null;
            }

            //respawn now: send request to the server
            _gameManager.ui.DisableDeath();
            _gameManager.localPlayerController.RPC_Respawn();
        }
        
        public bool PlayerCanRespawnFreely(PlayerController player)
        {
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