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
                
                if (_teamIndex == -1)
                {
                    SetEntityColor(Color.white);
                }
                else
                {
                    TeamInstance teamInstance = GameManager.TeamController.GetTeamByIndex(_teamIndex);

                    if (teamInstance != null && teamInstance.teamDefinition != null)
                    {
                        TeamDefinition teamDefinition = teamInstance.teamDefinition;
                        SetEntityColor(teamDefinition.TeamColorPrim);
                    }
                }
            }
        }
    }
}