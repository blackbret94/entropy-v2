using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameMode
{
    public class MatchTimer : MonoBehaviour
    {
        public int maxTime = 120;
        
        private float _timerRefreshRate = .25f;
        private float _lastUpdateTime;
        private bool _matchTimerIsRunning = true;

        private double _startTime;
        
        public void InitTimer()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable CustomValue = new ExitGames.Client.Photon.Hashtable();
                _startTime = PhotonNetwork.Time;
                CustomValue.Add("StartTime", _startTime);
                PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
            }
            else
            {
                _startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            }
        }
        
        public int CurrentMatchTime()
        {
            double time = PhotonNetwork.Time - _startTime;
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
                if (PhotonNetwork.IsMasterClient)
                {
                    if (CurrentMatchTime() <= 0 && _matchTimerIsRunning)
                    {
                        // End match
                        _matchTimerIsRunning = false;
                        GameManager gameManager = GameManager.GetInstance();
                        int teamWithHighestScore = gameManager.GetTeamWithHighestScore();
                        gameManager.localPlayer.photonView.RPC("RpcGameOver", RpcTarget.All,
                            (byte)teamWithHighestScore);
                    }
                }

                _lastUpdateTime = Time.time;
            }
        }
    }
}