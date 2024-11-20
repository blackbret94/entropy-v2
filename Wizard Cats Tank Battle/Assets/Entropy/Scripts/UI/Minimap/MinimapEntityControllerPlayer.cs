using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Minimap
{
    public class MinimapEntityControllerPlayer : MinimapEntityController
    {
        // public Color DeadColor = Color.gray;
        
        public Player Player;
        private float _refreshRate = .5f;
        private float _lastRefreshTime;

        // private bool _isLocalPlayer;
        private int _teamIndex = -1;

        private GameManager GameManager;
        // private bool _renderedAsAlive;

        protected override void Init()
        {
            base.Init();
            
            GameManager = GameManager.GetInstance();

            // _isLocalPlayer = Player.IsLocal;
            
            // if (_isLocalPlayer)
            // {
                // _entity.SetActiveIcon(false);
            // }
        }
        
        private void Update()
        {
            // if (_isLocalPlayer)
            // {
                // return;
            // }

            if (Time.time >= _lastRefreshTime + _refreshRate)
            {
                UpdateImage();
                _lastRefreshTime = Time.time;
            }
        }

        private void UpdateImage()
        {
            int teamIndex = Player.GetTeam();
            
            if (teamIndex != _teamIndex)
            {
                _teamIndex = teamIndex;
                RenderAsAlive();
            }
            
            SetOffscreenVisible(SameTeamAsLocalPlayer());
        }

        private bool SameTeamAsLocalPlayer()
        {
            Player localPlayer = Player.GetLocalPlayer();
            return localPlayer.GetTeam() == Player.GetTeam();
        }

        public void RenderAsAlive()
        {
            Team team = GameManager.GetTeamByIndex(_teamIndex);

            if (team == null)
                return;
            
            TeamDefinition teamDefinition = team.teamDefinition;

            SetEntityColor(teamDefinition.TeamColorPrim);
        }

        public void RenderAsDead()
        {
            SetEntityColor(Color.grey);
        }
    }
}