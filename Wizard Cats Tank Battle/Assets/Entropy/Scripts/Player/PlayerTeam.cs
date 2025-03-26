using Entropy.Scripts.Player;
using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.GameState;

namespace Vashta.Entropy.Player
{
    public class PlayerTeam : NetworkBehaviour
    {
        [Networked, OnChangedRender(nameof(OnTeamIdChanged))] 
        public int TeamIndex { get; set; }
        [Networked]
        public int PreferredTeamIndex { get; set; }

        public TeamController TeamController { get; private set; }
        public PlayerController PlayerController { get; private set; }
        public GameManager GameManager { get; private set; }
        public PlayerViewController PlayerViewController { get; private set; }

        private bool _hasInit = false;

        private void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
            PlayerViewController = GetComponent<PlayerViewController>();
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_hasInit)
                return;

            GameManager = GameManager.GetInstance();
            TeamController = GameManager.TeamController;
            
            _hasInit = true;
        }

        public void Setup()
        {
            Init();
            if (HasStateAuthority)
            {
                // Local, choose initial team
                TeamController.AttemptToChangePlayerToPreferredTeam(PlayerController);
                Debug.Log("Chose initial team: " + TeamIndex);
            }
            else
            {
                // Remote, apply networked values
                OnTeamIdChanged();
            }
        }

        public void SetPlayerPreferredTeam(int teamIndex, bool changeTeamsNow = false, bool respawnPlayer = false)
        {
            PreferredTeamIndex = teamIndex;
            // if (changeTeamsNow)
            // {
            //     TryChangeTeams(respawnPlayer);
            // }
        }
        
        public void OnTeamIdChanged()
        {
            if (TeamIndex == -1)
                return;
            
            Init();
            TeamController.ReCalculateTeams();
            
            ApplyTeamChange();
        }
        
        public void TryChangeTeams(bool respawn)
        {
            Init();
            
            Debug.Log("Attempting to change teams");

            if (respawn)
            {
                PlayerController.CombatController.RPCKillPlayerForRespawn();
                return;
            }
            
            if (GameManager.SpawnController.PlayerCanRespawnFreely(PlayerController) || !PlayerController.IsAlive)
            {
                TeamController.OnePassPlayerCheckToChangeTeams(PlayerController, respawn);
            }
        }
        
        // Handle team change, should re-color player
        // Most of TeamController stuff should be handled using the
        // synced network property there
        public void ApplyTeamChange()
        {
            if (TeamIndex == -1)
                return;

            Init();
            
            PlayerViewController.ColorizePlayerForTeam();
            GameManager.ui.GameLogPanel.EventPlayerChangedTeam(PlayerController.PlayerName, PlayerController.GetTeamDefinition());
        }
    }
}