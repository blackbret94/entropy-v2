using Fusion;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameMode
{
    public class MatchTimer : NetworkBehaviour
    {
        [Networked] public double StartTime { get; private set; }
        public int maxTime = 120;
        
        private float _timerRefreshRate = .25f;
        private float _lastUpdateTime;
        private bool _matchTimerIsRunning = true;
        
        public void InitTimer()
        {
            // If the room was just created, save the start time
            if (Runner.IsSharedModeMasterClient)
            {
                StartTime = Runner.SimulationTime;
            }
        }
        
        public int CurrentMatchTime()
        {
            double time = Runner.SimulationTime - StartTime;
            int timeRounded = System.Convert.ToInt32(System.Math.Floor(time));
            return Mathf.Max(0, maxTime - timeRounded);
        }

        public bool MatchTimeIsRunning()
        {
            return _matchTimerIsRunning;
        }

        public void StopTimer()
        {
            _matchTimerIsRunning = false;
        }
        
        private void Update()
        {
            // slow update
            if (Time.time > _lastUpdateTime + _timerRefreshRate)
            {
                if (CurrentMatchTime() <= 0 && _matchTimerIsRunning)
                {
                    // End match
                    _matchTimerIsRunning = false;
                    GameManager gameManager = GameManager.GetInstance();
                    int teamWithHighestScore = gameManager.TeamController.GetTeamWithHighestScore();
                    gameManager.GameOverController.GameOver((byte)teamWithHighestScore);
                }

                _lastUpdateTime = Time.time;
            }
        }
    }
}