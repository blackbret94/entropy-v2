using Photon.Pun;
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
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            
            int teamIndexInt = teamIndex;
            if (teamIndexInt == 255)
                teamIndexInt = -1;
            
            //display game over window
            _gameManager.GameOverController.DisplayGameOver(teamIndexInt);
        }
        
        // Server only
        private void AttemptToChangeToPreferredTeam(Player player, bool respawn)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            PhotonView view = player.photonView;
            int preferredTeamIndex = view.GetPreferredTeamIndex();
            int currentTeam = view.GetTeam();

            if (preferredTeamIndex == PlayerExtensions.RANDOM_TEAM_INDEX && preferredTeamIndex != currentTeam)
            {
                view.SetPreferredTeamIndex(currentTeam);
                return;
            }

            if (preferredTeamIndex == PlayerExtensions.RANDOM_TEAM_INDEX || preferredTeamIndex == view.GetTeam() ||
                !_gameManager.TeamController.TeamHasVacancy(preferredTeamIndex))
            {
                return;
            }

            Debug.Log("Changing teams to: " + preferredTeamIndex);

            PhotonNetwork.CurrentRoom.AddSize(view.GetTeam(), -1);
            view.SetTeam(preferredTeamIndex);
            PhotonNetwork.CurrentRoom.AddSize(view.GetTeam(), 1);
            
            // Force respawn
            if(respawn)
                player.CmdRespawn();
            
            view.RPC("RpcChangeTeams", RpcTarget.All);
        }
        
        // Server-only
        public void OnePassCheckChangeTeams(Player player, bool respawn)
        {
            if (!PhotonNetwork.IsMasterClient || !player)
                return;

            PhotonView view = player.photonView;
            int preferredTeamIndex = view.GetPreferredTeamIndex();
            bool prefersDifferentTeam = preferredTeamIndex != view.GetTeam();

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