using System.Collections.Generic;
using Entropy.Scripts.Player;
using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameMode
{
    public class KingOfTheHillController : MonoBehaviour
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
        
        private void Update()
        {
            Init();
            
            // Only run if game mode is KOTH
            TanksMP.GameMode gameMode = GameManager.GameModeDefinition.GameMode;
            
            if (gameMode == TanksMP.GameMode.KOTH || gameMode == TanksMP.GameMode.KOTHS)
            {
                if (_timerIsRunning && _lastTick + TickTimeS < Time.time)
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

                    // Server only
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // Award points to teamControllingPoint
                        if (teamControllingPoint != -1)
                            GameManager.ScoreController.AddScore(ScoreType.HoldPoint, teamControllingPoint);
                    }
                }
            }
            
            _lastTick = Time.time;
            
            // SERVER ONLY check for game over
            if (PhotonNetwork.IsMasterClient)
            {
                if (GameManager.GetInstance().ScoreController.IsGameOver())
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;

                    int teamWithHighestScore = GameManager.ScoreController.GetTeamWithHighestScore();
                    
                    PlayerList.GetLocalPlayer().photonView.RPC("RpcGameOver", RpcTarget.All, (byte)teamWithHighestScore);
        
                    _timerIsRunning = false;
                    _hasCalledGameOver = true;
                }
            }
        }
    }
}