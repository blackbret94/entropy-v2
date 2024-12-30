using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameState
{
    public class ScoreController : MonoBehaviour
    {
        private GameManager _gameManager;
        public int maxScore { get; set; }= 30;
        
        private void Awake()
        {
            _gameManager = GetComponent<GameManager>();
        }
        
        /// <summary>
        /// Adds points to the target team depending on matching game mode and score type.
        /// This allows us for granting different amount of points on different score actions.
        /// </summary>
        public void AddScore(ScoreType scoreType, int teamIndex)
        {
            GameModeDefinition gameMode = _gameManager.GameModeDefinition;
            
            switch(scoreType)
            {
                case ScoreType.Kill:
                    PhotonNetwork.CurrentRoom.AddScore(teamIndex, gameMode.KillPoints);
                    break;
                
                case ScoreType.Capture:
                    PhotonNetwork.CurrentRoom.AddScore(teamIndex, gameMode.CapturePoints);
                    break;
                
                case ScoreType.HoldPoint:
                    PhotonNetwork.CurrentRoom.AddScore(teamIndex, gameMode.HoldPointPoints);
                    break;
            }
        }

        public void RemoveScore(ScoreType scoreType, int teamIndex)
        {
            PhotonNetwork.CurrentRoom.AddScore(teamIndex, -1);
        }
        
        /// <summary>
        /// Returns whether a team reached the maximum game score.
        /// </summary>
        public bool IsGameOver()
        {
            if (!_gameManager.MatchTimer.MatchTimeIsRunning())
                return true;
            
            //init variables
            bool isOver = false;
            int[] score = PhotonNetwork.CurrentRoom.GetScore();
            
            //loop over teams to find the highest score
            for(int i = 0; i < _gameManager.TeamController.teams.Length; i++)
            {
                //score is greater or equal to max score,
                //which means the game is finished
                if(score[i] >= _gameManager.ScoreController.maxScore)
                {
                    isOver = true;
                    break;
                }
            }
            
            //return the result
            return isOver;
        }

        public int GetTeamWithHighestScore()
        {
            int[] score = PhotonNetwork.CurrentRoom.GetScore();
            int teamWithHighestScore = -1;
            int highestScoreFound = 0;
            
            //loop over teams to find the highest score
            for(int i = 0; i < _gameManager.TeamController.teams.Length; i++)
            {
                if(score[i] > highestScoreFound)
                {
                    highestScoreFound = score[i];
                    teamWithHighestScore = i;
                    
                } else if (score[i] == highestScoreFound)
                {
                    teamWithHighestScore = -1;
                }
            }

            return teamWithHighestScore;
        }
    }
}