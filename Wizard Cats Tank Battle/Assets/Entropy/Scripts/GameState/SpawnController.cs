using System.Collections;
using TanksMP;
using UnityEngine;
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
        /// If Unity Ads is enabled, tries to show an ad during the respawn delay.
        /// By using the 'skipAd' parameter is it possible to force skipping ads.
        /// </summary>
        public void DisplayDeath(bool skipAd = false)
        {
            if (!ClassSelectionPanel.Instance.CountdownIsActive())
            {
                Player localPlayer = _gameManager.localPlayer;
                //get the player component that killed us
                Player other = localPlayer;
                string killedByName = "YOURSELF";
                if (localPlayer.killedBy != null)
                    other = localPlayer.killedBy.GetComponent<Player>();

                //suicide or regular kill?
                if (other != localPlayer)
                {
                    killedByName = other.GetView().GetName();
                }

                //calculate if we should show a video ad
#if UNITY_ADS
            if (!skipAd && UnityAdsManager.ShowAd())
                return;
#endif

                //when no ad is being shown, set the death text
                //and start waiting for the respawn delay immediately
                _gameManager.ui.SetDeathText(killedByName, _gameManager.TeamController.teams[other.GetView().GetTeam()]);
            }

            StartCoroutine(SpawnRoutine());
        }


        //coroutine spawning the player after a respawn delay
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
            _gameManager.localPlayer.CmdRespawn();
        }
    }
}