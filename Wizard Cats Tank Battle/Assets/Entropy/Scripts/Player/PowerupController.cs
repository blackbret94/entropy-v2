using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.Player
{
    public class PowerupController : NetworkBehaviour
    {
        public StatusEffectDirectory StatusEffectDirectory;
        
        public int PowerupId { get; protected set; }
        
        [Header("Cached references")] 
        private PlayerController _playerController;

        private bool _hasInit;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if(_hasInit) return;
            _playerController = GetComponent<PlayerController>();
            _hasInit = true;
        }

        public void SetPowerupId(int powerupId)
        {
            PowerupId = powerupId;
        }
        
        public void TryCastPowerup()
        {
            Init();

            if (PowerupId > 0)
            {
                RPC_CastPowerup(PowerupId);
            }
            else
            {
                // Debug.LogWarning("Tried to cast powerup with ID <=0: "+ PowerupId);
            }
        }
        
        /// <summary>
        /// Passes powerupId as a parameter, only the user has to know their powerup.
        /// </summary>
        /// <param name="powerupId"></param>
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
        private void RPC_CastPowerup(int powerupId)
        {
            if (powerupId < 1)
            {
                Debug.LogError("Could not cast powerup, session ID: " + powerupId);
            }
            
            StatusEffectData data = StatusEffectDirectory.GetBySessionId(powerupId);

            if (!data)
            {
                Debug.LogError("Could not find powerup, session ID: " + powerupId);
            }
            
            _playerController.StatusEffectController.AddStatusEffect(data.Id, _playerController);
            
            if (HasInputAuthority)
            {
                UIGame.GetInstance().CastPowerupButton.ClosePanel();
            }

            // This might be re-setting it before the client has a chance to read it.
            PowerupId = 0;
        }
    }
}