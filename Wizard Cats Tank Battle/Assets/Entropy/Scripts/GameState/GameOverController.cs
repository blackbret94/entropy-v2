using System.Collections;
using Entropy.Scripts.Audio;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.GameMode;

namespace Vashta.Entropy.GameState
{
    public class GameOverController : MonoBehaviour
    {
        private GameManager _gameManager;

        private void Awake()
        {
            _gameManager = GetComponent<GameManager>();
        }
        
        /// <summary>
        /// Only for this player: sets game over text stating the winning team.
        /// Disables player movement so no updates are sent through the network.
        /// </summary>
        public void DisplayGameOver(int teamIndex)
        {
            _gameManager.MatchTimer.StopTimer();
            _gameManager.localPlayer.enabled = false;
            if (_gameManager.localPlayer.CameraController)
            {
                _gameManager.localPlayer.CameraController.HideMask(true);
            }
            
            if (teamIndex != -1)
            {
                Team winningTeam = _gameManager.TeamController.teams[teamIndex];
                _gameManager.ui.SetGameOverText(winningTeam);
            }
            else
            {
                _gameManager.ui.SetGameOverText(null);
            }

            //starts coroutine for displaying the game over window
            StopCoroutine(_gameManager.SpawnController.SpawnRoutine());
            StartCoroutine(DisplayGameOverCR(teamIndex));
        }

        //displays game over window after short delay
        IEnumerator DisplayGameOverCR(int teamIndex)
        {
            //give the user a chance to read which team won the game
            //before enabling the game over screen
            yield return new WaitForSeconds(3);

            //show game over window (still connected at that point)
            if (teamIndex != -1)
            {
                // Handle victory
                Team winningTeam = _gameManager.TeamController.teams[teamIndex];
                _gameManager.ui.ShowGameOver(teamIndex, winningTeam.name, winningTeam.material.color);

                int playerTeamIndex = _gameManager.localPlayer.GetView().GetTeam();
                if (playerTeamIndex == teamIndex)
                    _gameManager.MusicController.PlayVictoryMusic();
                else
                    _gameManager.MusicController.PlayDefeatMusic();
            }
            else
            {
                // Handle tie
                _gameManager.ui.ShowGameOver(teamIndex, "NO ONE", Color.white);
                _gameManager.MusicController.PlayDefeatMusic();
            }
        }
    }
}