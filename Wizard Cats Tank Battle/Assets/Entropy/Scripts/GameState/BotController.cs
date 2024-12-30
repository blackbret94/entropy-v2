using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameState
{
    public class BotController : MonoBehaviour
    {
        /// <summary>
        /// Amount of bots to spawn across all teams.
        /// </summary>
        public int maxBots;
        
        /// <summary>
        /// Selection of bot prefabs to choose from.
        /// </summary>
        public GameObject[] prefabs;
        
        public List<GameObject> BotTargetList;
        private List<PlayerBot> _botList;

        private void Awake()
        {
            _botList = new List<PlayerBot>();
            
            //disabled when not in offline mode
            if ((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode) != NetworkMode.Offline)
                this.enabled = false;
        }
        
        IEnumerator Start()
        {
            //wait a second for all script to initialize
            yield return new WaitForSeconds(1);

            //loop over bot count
            for(int i = 0; i < maxBots; i++)
            {
                //randomly choose bot from array of bot prefabs
                //spawn bot across the simulated private network
                int randIndex = Random.Range(0, prefabs.Length);
                GameObject obj = PhotonNetwork.Instantiate(prefabs[randIndex].name, Vector3.zero, Quaternion.identity, 0);

                //let the local host determine the team assignment
                Player p = obj.GetComponent<Player>();
                p.GetView().SetTeam(GameManager.GetInstance().TeamController.GetTeamFill());

                //increase corresponding team size
                PhotonNetwork.CurrentRoom.AddSize(p.GetView().GetTeam(), +1);

                yield return new WaitForSeconds(0.25f);
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