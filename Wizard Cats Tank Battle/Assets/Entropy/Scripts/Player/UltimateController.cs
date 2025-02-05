using UnityEngine;
using Vashta.Entropy.Player;

namespace Entropy.Scripts.Player
{
    public class UltimateController : MonoBehaviour
    {
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

        // Server only
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
            int ultimateCost = _playerController.GetClass().ultimateCost;
            
            if (Ultimate >= ultimateCost)
            {
                _playerController.CastUltimate();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}