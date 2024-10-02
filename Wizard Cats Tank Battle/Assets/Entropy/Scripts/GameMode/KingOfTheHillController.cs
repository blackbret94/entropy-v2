using System.Collections.Generic;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameMode
{
    public class KingOfTheHillController : MonoBehaviour
    {
        public List<ControlPoint> ControlPoints;
        public float TickTimeS = 1f;
        public GameManager GameManager;

        private float _lastTick;

        private void Update()
        {
            // Only run if game mode is KOTH
            if (GameManager.GetGameModeDefinition().GameMode == TanksMP.GameMode.KOTH)
            {
                if (_lastTick + TickTimeS < Time.time)
                {
                    OneTick();
                }
            }
        }

        private void OneTick()
        {
            foreach (ControlPoint controlPoint in ControlPoints)
            {
                if (controlPoint != null)
                {
                    // Refresh state
                    controlPoint.OneTickCapture();
                    int teamControllingPoint = controlPoint.ControlledByTeamIndex;
                    
                    // Award points to teamControllingPoint
                    if(teamControllingPoint != -1)
                        GameManager.AddScore(ScoreType.HoldPoint, teamControllingPoint);
                }
            }

            _lastTick = Time.time;
        }
    }
}