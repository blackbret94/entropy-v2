using Fusion;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.Spells;

namespace Entropy.Scripts.Player
{
    public class UltimateController : NetworkBehaviour
    {
        // Not networked because only the local player needs to know this value
        public int Ultimate { get; private set; }
        
        [Header("Cached references")] 
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }
        
        // Server only
        public void IncreaseUltimate()
        {
            // Only give to living players
            if (!_playerController.IsAlive)
                return;

            int ultimateCost = _playerController.GetClass().ultimateCost;
            if (Ultimate < ultimateCost)
                Ultimate++;
        }

        // Server only
        public void RewardUltimateForKill()
        {
            // Only give to living players
            if (!_playerController.IsAlive)
                return;

            int ultimateIncrease = 5;
            int ultimateCost = _playerController.GetClass().ultimateCost;

            if (Ultimate < ultimateCost)
            {
                Ultimate += ultimateIncrease;
            }
        }
        
        public void ClearUltimate()
        {
            Ultimate = 0;
        }

        public float GetUltimatePerun()
        {
            int ultimateCost = _playerController.GetClass().ultimateCost;
            return (float)Ultimate / ultimateCost;
        }

        /// <summary>
        /// Called by local player
        /// </summary>
        /// <returns></returns>
        public bool TryCastUltimate()
        {
            ClassDefinition playerClass = _playerController.GetClass();
            int ultimateCost = playerClass.ultimateCost;
            
            if (Ultimate >= ultimateCost)
            {
                SpellData ultimateSpell = playerClass.ultimateSpell;
                ClearUltimate();
            
                if (!ultimateSpell)
                {
                    Debug.LogError("Class with ID " + playerClass.classId + " is missing an ultimate spell!");
                    return false;
                }

                RPC_CastUltimate();
                
                return true;
            }
            else
            {
                return false;
            }
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
        public void RPC_CastUltimate()
        {
            ClassDefinition playerClass = _playerController.GetClass();
            SpellData ultimateSpell = playerClass.ultimateSpell;
            ultimateSpell.Cast(_playerController);
        }
    }
}