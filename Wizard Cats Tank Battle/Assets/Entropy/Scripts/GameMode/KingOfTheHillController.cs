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
        private bool _shouldRun = false;
        
        public override void Spawned()
        {
            Init();
        }
        
        private bool Init()
        {
            if (!GameManager.HasSpawned)
                return false;
            
            if (_hasInit) return true;
            
            TanksMP.GameMode gameMode = GameManager.MatchInfo.GameMode;

            if (gameMode == TanksMP.GameMode.KOTH)
            {
                ControlPoints = ControlPointsSingle;
                _shouldRun = true;
            } else if (gameMode == TanksMP.GameMode.KOTHS)
            {
                ControlPoints = ControlPointsMulti;
                _shouldRun = true;
            }

            _hasInit = true;
            return true;
        }
        
        public override void FixedUpdateNetwork()
        {
            if (!Init() || !_shouldRun)
                return;
            
            if (_timerIsRunning && _lastTick + TickTimeS < Runner.SimulationTime)
            {
                OneTick();
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
                    if (HasStateAuthority && teamControllingPoint != -1)
                        GameManager.TeamController.RPCAddScore(ScoreType.HoldPoint, teamControllingPoint);
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