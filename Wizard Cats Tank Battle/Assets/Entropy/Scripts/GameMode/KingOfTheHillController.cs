using System.Collections.Generic;
using Fusion;
using TanksMP;

namespace Vashta.Entropy.GameMode
{
    public class KingOfTheHillController : NetworkBehaviour
    {
        public List<ControlPoint> ControlPointsSingle;
        public List<ControlPoint> ControlPointsMulti;
        
        private List<ControlPoint> ControlPoints;
        public float TickTimeS = 1f;
        public GameManager GameManager;

        private float _lastTick;
        private bool _timerIsRunning = true;

        private bool _hasInit;
        private bool _hasCalledGameOver = false;

        public override void Spawned()
        {
            Init();
        }
        
        private void Init()
        {
            if (_hasInit) return;
            
            TanksMP.GameMode gameMode = GameManager.GameModeDefinition.GameMode;

            if (gameMode == TanksMP.GameMode.KOTH)
            {
                ControlPoints = ControlPointsSingle;
            } else if (gameMode == TanksMP.GameMode.KOTHS)
            {
                ControlPoints = ControlPointsMulti;
            }

            _hasInit = true;
        }
        
        public override void FixedUpdateNetwork()
        {
            Init();
            
            // Only run if game mode is KOTH
            TanksMP.GameMode gameMode = GameManager.GameModeDefinition.GameMode;
            
            if (gameMode == TanksMP.GameMode.KOTH || gameMode == TanksMP.GameMode.KOTHS)
            {
                if (_timerIsRunning && _lastTick + TickTimeS < Runner.SimulationTime)
                {
                    OneTick();
                }
            }
        }

        private void OneTick()
        {
            if (_hasCalledGameOver)
                return;
            
            foreach (ControlPoint controlPoint in ControlPoints)
            {
                if (controlPoint != null)
                {
                    // Refresh state
                    controlPoint.OneTickCapture();
                    int teamControllingPoint = controlPoint.ControlledByTeamIndex;
                    
                    // Award points to teamControllingPoint
                    if (teamControllingPoint != -1)
                        GameManager.TeamController.AddScore(ScoreType.HoldPoint, teamControllingPoint);
                }
            }
            
            _lastTick = Runner.SimulationTime;
            
            if (GameManager.IsGameOver())
            {
                GameManager.Runner.SessionInfo.IsOpen = false;

                int teamWithHighestScore = GameManager.TeamController.GetTeamWithHighestScore();
                
                GameManager.GameOverController.RPCGameOver((byte)teamWithHighestScore);
    
                _timerIsRunning = false;
                _hasCalledGameOver = true;
            }
        }
    }
}