using Entropy.Scripts.Player;
using TanksMP;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Minimap
{
    public class MinimapEntityControllerPlayer : MinimapEntityController
    {
        // public Color DeadColor = Color.gray;
        
        [FormerlySerializedAs("Player")] public PlayerController playerController;
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
            int teamIndex = playerController.TeamIndex;
            
            if (teamIndex != _teamIndex)
            {
                _teamIndex = teamIndex;
                RenderAsAlive();
            }
            
            SetOffscreenVisible(SameTeamAsLocalPlayer());
        }

        private bool SameTeamAsLocalPlayer()
        {
            PlayerController localPlayerController = PlayerList.GetLocalPlayer();

            if (localPlayerController == null)
            {
                return false;
            }
            
            return localPlayerController.TeamIndex == playerController.TeamIndex;
        }

        public void RenderAsAlive()
        {
            TeamInstance teamInstance = GameManager.TeamController.GetTeamByIndex(playerController.TeamIndex);

            if (teamInstance == null)
                return;
            
            TeamDefinition teamDefinition = teamInstance.teamDefinition;

            SetEntityColor(teamDefinition.TeamColorPrim);
        }

        public void RenderAsDead()
        {
            SetEntityColor(Color.grey);
        }
    }
}