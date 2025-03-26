using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.GameState
{
    public class BotController : NetworkBehaviour
    {
        /// <summary>
        /// Amount of bots to spawn across all teams.
        /// </summary>
        public int maxBots;
        
        /// <summary>
        /// Selection of bot prefabs to choose from.
        /// </summary>
        public GameObject prefab;
        
        public List<GameObject> BotTargetList;
        private List<PlayerControllerBot> _botList;
        private GameManager _gameManager;

        public override void Spawned()
        {
            base.Spawned();
            _gameManager = GameManager.GetInstance();
            _botList = new List<PlayerControllerBot>();
            
            if(Runner.GameMode == Fusion.GameMode.Single && _gameManager.HasStateAuthority)
            {
                StartCoroutine(SpawnBots());
            }
            else
            {
                // Eventually, this should instead use a "maxBotCount" property
            }
        }
        
        private IEnumerator SpawnBots()
        {
            //wait a second for all script to initialize
            yield return new WaitForSeconds(1);

            //loop over bot count
            for (int i = 0; i < maxBots; i++)
            {
                //randomly choose bot from array of bot prefabs
                //spawn bot across the simulated private network
                NetworkObject obj = Runner.Spawn(prefab, Vector3.zero, Quaternion.identity);

                //let the local host determine the team assignment
                // PlayerController p = obj.GetComponent<PlayerController>();
                // int teamIndex = GameManager.GetInstance().TeamController.GetTeamFill();
                // p.PlayerTeam.SetPlayerPreferredTeam(teamIndex);
                // p.PlayerTeam.TryChangeTeams(true);

                //increase corresponding team size
                // _gameManager.TeamController.AddPlayerTeamTeam(p, p.TeamIndex);

                yield return new WaitForSeconds(0.33f);
            }
        }
        
        public void AddBot(PlayerControllerBot controllerBot)
        {
            _botList.Add(controllerBot);
        }

        public List<PlayerControllerBot> GetBotList()
        {
            return _botList;
        }
    }
}