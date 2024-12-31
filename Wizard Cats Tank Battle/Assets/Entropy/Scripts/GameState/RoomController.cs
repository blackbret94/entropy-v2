using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameState
{
    public class RoomController : MonoBehaviour
    {
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.GetInstance();
        }
        
        // Server only
        public void GameOver(byte teamIndex)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            
            int teamIndexInt = teamIndex;
            if (teamIndexInt == 255)
                teamIndexInt = -1;
            
            //display game over window
            _gameManager.GameOverController.DisplayGameOver(teamIndexInt);
        }
        
        // Server only
        private void AttemptToChangeToPreferredTeam(Player player, bool respawn)
        {
            int preferredTeamIndex = player.PreferredTeamIndex;
            int currentTeam = player.TeamIndex;

            if (preferredTeamIndex == PlayerExtensions.RANDOM_TEAM_INDEX && preferredTeamIndex != currentTeam)
            {
                player.PreferredTeamIndex = currentTeam;
                return;
            }

            if (preferredTeamIndex == PlayerExtensions.RANDOM_TEAM_INDEX || preferredTeamIndex == currentTeam ||
                !_gameManager.TeamController.TeamHasVacancy(preferredTeamIndex))
            {
                return;
            }

            Debug.Log("Changing teams to: " + preferredTeamIndex);

            PhotonNetwork.CurrentRoom.AddSize(player.TeamIndex, -1);
            player.TeamIndex = preferredTeamIndex;
            PhotonNetwork.CurrentRoom.AddSize(player.TeamIndex, 1);
            
            // Force respawn
            if(respawn)
                player.Respawn(null);
            
            player.ApplyTeamChange();
        }
        
        // Server-only
        public void OnePassCheckChangeTeams(Player player, bool respawn)
        {
            if (!player)
                return;
            
            int preferredTeamIndex = player.PreferredTeamIndex;
            bool prefersDifferentTeam = preferredTeamIndex != player.TeamIndex;

            if (prefersDifferentTeam)
            {
                Debug.Log("Prefers a different team");
                if (_gameManager.TeamController.TeamHasVacancy(preferredTeamIndex))
                {
                    // Handle game over. Nested for efficiency
                    if (_gameManager.ScoreController.IsGameOver())
                        return;

                    AttemptToChangeToPreferredTeam(player, respawn);
                }
            }
        }
    }
}