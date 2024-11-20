using System;
using System.Collections;
using TanksMP;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class InitMinimap : MonoBehaviour
    {
        private TanksMP.Player _localPlayer;
        public bl_MiniMap MiniMap;
        private float _refreshRate = .5f;
        private float _lastRefreshTime;

        private int _teamIndex = -1;
        
        private void Start()
        {
            StartCoroutine(AttachToPlayerCoroutine());
        }

        private void Update()
        {
            if (!_localPlayer)
                return;
            
            if (Time.time >= _lastRefreshTime + _refreshRate)
            {
                UpdateImage();
                _lastRefreshTime = Time.time;
            }
        }

        private IEnumerator AttachToPlayerCoroutine()
        {
            while (_localPlayer == null)
            {
                _localPlayer = TanksMP.Player.GetLocalPlayer();
                yield return null;
            }

            MiniMap.Target = _localPlayer.transform;
            Debug.Log("Player attached!");
        }

        private void UpdateImage()
        {
            if (MiniMap == null)
                return;
            
            int teamIndex = _localPlayer.GetTeam();

            if (teamIndex != _teamIndex)
            {
                _teamIndex = teamIndex;
                Team team = GameManager.GetInstance().GetTeamByIndex(_teamIndex + 1);

                try
                {
                    if (team != null && team.teamDefinition != null)
                    {
                        MiniMap.SetPointerColor(team.teamDefinition.TeamColorPrim);
                    }
                }
                catch (NullReferenceException e)
                {
                    Debug.LogWarning(e);
                }
            }
        }
    }
}