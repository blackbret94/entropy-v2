using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.StatusEffects;

namespace Entropy.Scripts.Player
{
    public class DeathController : NetworkBehaviour
    {
        [Networked, OnChangedRender(nameof(OnPlayerDeathChanged))]
        public PlayerDeathStruct DeathStruct { get; set; }
        
        private PlayerController _playerController;
        private StatusEffectController _statusEffectController;
        private GameManager _gameManager;
        
        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            _statusEffectController = _playerController.StatusEffectController;
            _gameManager = GameManager.GetInstance();
        }
        
        private void OnPlayerDeathChanged()
        {
            if (_playerController.IsAlive && !DeathStruct.expired)
            {
                // Handle death
                PlayerController otherPlayer = PlayerList.GetPlayer(DeathStruct.killedByPlayer);
                KillPlayer(otherPlayer, DeathStruct.visualEffectId);
            }
        }

        public void KillPlayerLocalDamage(PlayerController other, ushort deathFxId = 0)
        {
            DeathStruct = new PlayerDeathStruct(other != null ? other.NetID : (short)-1, deathFxId, Runner.SimulationTime);

            KillPlayer(other, deathFxId);
        }
        
        // A simple command that ignores the player's health and just kills them.  Useful for respawning on class or team change.
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPCKillPlayerForRespawn()
        {
            _playerController.SetHealth(0);
            _playerController.SetShield(0);
            KillPlayer(null);
        }
        
        // The main Kill Player method
        private void KillPlayer(PlayerController other, ushort deathFxId = 0)
        {
            _playerController.MovementController.ResetTransform();
            _gameManager.TeamController.OnePassPlayerCheckToChangeTeams(_playerController, false);
            
            //get killer and increase score for that enemy team
            if (other != null)
            {
                // Reflect damage on killer if blood pact is active
                _statusEffectController.BloodPact(other);
                
                int otherTeam = other.TeamIndex;
                
                // killer is other team
                if (_playerController.TeamIndex != otherTeam)
                {
                    if (HasStateAuthority)
                    {
                        _gameManager.TeamController.RPCAddScore(ScoreType.Kill, otherTeam);
                    }

                    other.Kills++;
                }
                
                //the maximum score has been reached now
                if (_gameManager.IsGameOver())
                {
                    //tell all clients the winning team
                    _gameManager.GameOverController.RPCGameOver((byte)otherTeam);
                    // return;
                }
            }
            // else 
            // if(!_playerController.RespawnIsFreeFromJointime())
            // {
                // Killed by environment
                // _gameManager.TeamController.RemoveScore(ScoreType.Kill, _playerController.TeamIndex);
            // }
            
            // The game is not over
            _playerController.PlayerDeath(other, deathFxId);
        }
    }
}