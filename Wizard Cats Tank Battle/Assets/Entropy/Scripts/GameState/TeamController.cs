using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameState
{
    public class TeamController : MonoBehaviour
    {
        public const int RANDOM_TEAM_INDEX = 100;
        
        /// <summary>
        /// Definition of playing teams with additional properties.
        /// </summary>
        public Team[] teams;
        public bool UsesTeams => _gameManager.gameMode != TanksMP.GameMode.FFA;
        
        private GameManager _gameManager;
        private int lastSpawnIndex = -1;

        private void Awake()
        {
            _gameManager = GetComponent<GameManager>();
        }
        
        public Team GetTeamByIndex(int index)
        {
            if (index < teams.Length && index >= 0)
            {
                return teams[index];
            }

            return teams[0];
        }
        
        /// <summary>
        /// Returns the next team index a player should be assigned to.
        /// </summary>
        public int GetTeamFill()
        {
            //init variables
            int[] size = PhotonNetwork.CurrentRoom.GetSize();
            int teamNo = 0;

            int min = size[0];
            //loop over teams to find the lowest fill
            for (int i = 0; i < teams.Length; i++)
            {
                //if fill is lower than the previous value
                //store new fill and team for next iteration
                if (size[i] < min)
                {
                    min = size[i];
                    teamNo = i;
                }
            }

            //return index of lowest team
            return teamNo;
        }

        public bool TeamHasVacancy(int teamIndex)
        {
            return true;
            // TODO: Revisit this later
            // int teamNo = teamIndex - 1;
            int maxTeamSize = 3; // This should NOT be hardcoded here
            
            int[] size = PhotonNetwork.CurrentRoom.GetSize();

            return size[teamIndex] < maxTeamSize;
        }
        
        /// <summary>
        /// Returns a random spawn position within the team's spawn area.
        /// </summary>
        public Vector3 GetSpawnPosition(int teamIndex)
        {
            return UsesTeams ? GetSpawnTeams(teamIndex) : GetSpawnFFA();
        }
        
        private Vector3 GetSpawnTeams(int teamIndex)
        {
            //init variables
            Vector3 pos = teams[teamIndex].spawn.position;
            BoxCollider col = teams[teamIndex].spawn.GetComponent<BoxCollider>();

            if(col != null)
            {
                //find a position within the box collider range, first set fixed y position
                //the counter determines how often we are calculating a new position if out of range
                pos.y = col.transform.position.y;
                int counter = 10;
                
                //try to get random position within collider bounds
                //if it's not within bounds, do another iteration
                do
                {
                    pos.x = UnityEngine.Random.Range(col.bounds.min.x, col.bounds.max.x);
                    pos.z = UnityEngine.Random.Range(col.bounds.min.z, col.bounds.max.z);
                    counter--;
                }
                while(!col.bounds.Contains(pos) && counter > 0);
            }
            
            return pos;
        }

        private Vector3 GetSpawnFFA()
        {
            int counter = 10;
            int spawnIndex;
            do
            {
                spawnIndex = Random.Range(0, teams.Length);
            } while (lastSpawnIndex == spawnIndex && counter-- > 0);

            return GetSpawnTeams(spawnIndex);
        }
    }
}