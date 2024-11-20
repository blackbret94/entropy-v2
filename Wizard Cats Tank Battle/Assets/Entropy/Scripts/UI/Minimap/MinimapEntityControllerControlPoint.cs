using System.Collections;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.GameMode;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Minimap
{
    public class MinimapEntityControllerControlPoint : MinimapEntityController
    {
        private float _refreshRate = .5f;
        private float _lastRefreshTime;

        public ControlPoint ControlPoint;
        private GameManager GameManager;
        
        private int _teamIndex = -1;
        
        protected override void Init()
        {
            base.Init();
            GameManager = GameManager.GetInstance();
        }
        
        private void Update()
        {
            if (Time.time >= _lastRefreshTime + _refreshRate)
            {
                UpdateTeamColor();
                _lastRefreshTime = Time.time;
            }
        }

        private void UpdateTeamColor()
        {
            int teamIndex = ControlPoint.ControlledByTeamIndex;
                
            if (teamIndex != _teamIndex)
            {
                _teamIndex = teamIndex;
                Team team = GameManager.GetTeamByIndex(_teamIndex);

                if (team != null && team.teamDefinition != null)
                {
                    TeamDefinition teamDefinition = team.teamDefinition;
                    SetEntityColor(teamDefinition.TeamColorPrim);
                }
                else
                {
                    SetEntityColor(Color.white);
                }
            }
        }
    }
}