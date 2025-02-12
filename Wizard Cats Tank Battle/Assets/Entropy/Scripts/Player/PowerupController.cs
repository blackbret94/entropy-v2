using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.Player
{
    public class PowerupController : NetworkBehaviour
    {
        public StatusEffectDirectory StatusEffectDirectory;
        
        [Networked]
        public int PowerupId { get; protected set; }
        
        [Header("Cached references")] 
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void SetPowerupId(int powerupId)
        {
            PowerupId = powerupId;
        }
        
        public void TryCastPowerup()
        {
            if (PowerupId > 0)
            {
                RPC_CastPowerup();
            }
            else
            {
                Debug.LogWarning("Tried to cast powerup with ID <=0: "+ PowerupId);
            }
        }
        
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
        public void RPC_CastPowerup()
        {
            if (PowerupId < 1)
            {
                Debug.LogError("Could not cast powerup, session ID: " + PowerupId);
            }
            
            StatusEffectData data = StatusEffectDirectory.GetBySessionId(PowerupId);

            if (!data)
            {
                Debug.LogError("Could not find powerup, session ID: " + PowerupId);
            }
            
            _playerController.StatusEffectController.AddStatusEffect(data.Id, _playerController);
            
            if (HasInputAuthority)
            {
                UIGame.GetInstance().CastPowerupButton.ClosePanel();
            }

            PowerupId = 0;
        }
    }
}