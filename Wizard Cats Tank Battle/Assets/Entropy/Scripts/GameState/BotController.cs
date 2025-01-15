using System.Collections;
using System.Collections.Generic;
using Fusion;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameState
{
    public class BotController : SimulationBehaviour
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
        private List<PlayerBot> _botList;
        private GameManager _gameManager;

        private void Awake()
        {
            //temporarily disable
            this.enabled = false;
            
            _botList = new List<PlayerBot>();
            
            //disabled when not in offline mode
            if ((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode) != NetworkMode.Offline)
                this.enabled = false;
        }
        
        IEnumerator Start()
        {
            // Temp disabled
            if (false)
            {
                _gameManager = GameManager.GetInstance();

                //wait a second for all script to initialize
                yield return new WaitForSeconds(1);

                //loop over bot count
                for (int i = 0; i < maxBots; i++)
                {
                    //randomly choose bot from array of bot prefabs
                    //spawn bot across the simulated private network
                    NetworkObject obj = Runner.Spawn(prefab, Vector3.zero, Quaternion.identity);

                    //let the local host determine the team assignment
                    Player p = obj.GetComponent<Player>();
                    p.TeamIndex = GameManager.GetInstance().TeamController.GetTeamFill();

                    //increase corresponding team size
                    _gameManager.TeamController.AddPlayerTeamTeam(p, p.TeamIndex);

                    yield return new WaitForSeconds(0.25f);
                }
            }
        }
        
        public void AddBot(PlayerBot bot)
        {
            _botList.Add(bot);
        }

        public void ClearBots()
        {
            _botList.Clear();
        }

        public List<PlayerBot> GetBotList()
        {
            return _botList;
        }
    }
}