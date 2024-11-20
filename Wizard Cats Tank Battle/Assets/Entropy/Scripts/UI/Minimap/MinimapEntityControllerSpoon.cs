using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Minimap
{
    public class MinimapEntityControllerSpoon : MinimapEntityController
    {
        private float _refreshRate = .5f;
        private float _lastRefreshTime;
        private GameManager GameManager;
        private int _teamIndex = -1;
        
        public CollectibleCaptureTheFlag Spoon;

        protected override void Init()
        {
            base.Init();
            GameManager = GameManager.GetInstance();
        }
        
        private void Update()
        {
            if (Time.time >= _lastRefreshTime + _refreshRate)
            {
                ColorSpoon();
                _lastRefreshTime = Time.time;
            }
        }
        
        private void ColorSpoon()
        {
            int teamIndex = Spoon.teamIndex;

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