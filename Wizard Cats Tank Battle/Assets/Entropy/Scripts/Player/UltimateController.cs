using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class UltimateController : MonoBehaviour
    {
        public int Ultimate { get; private set; }
        
        [Header("Cached references")] 
        private TanksMP.Player _player;

        private void Awake()
        {
            _player = GetComponent<TanksMP.Player>();
        }
        
        // Server only
        public void IncreaseUltimate()
        {
            // Only give to living players
            if (!_player.IsAlive)
                return;

            int ultimateCost = _player.GetClass().ultimateCost;
            if (Ultimate < ultimateCost)
                Ultimate++;
        }

        // Server only
        public void RewardUltimateForKill()
        {
            // Only give to living players
            if (!_player.IsAlive)
                return;

            int ultimateIncrease = 5;
            int ultimateCost = _player.GetClass().ultimateCost;

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
            int ultimateCost = _player.GetClass().ultimateCost;
            return (float)Ultimate / ultimateCost;
        }

        /// <summary>
        /// Called by local player
        /// </summary>
        /// <returns></returns>
        public bool TryCastUltimate()
        {
            int ultimateCost = _player.GetClass().ultimateCost;
            
            if (Ultimate >= ultimateCost)
            {
                _player.CastUltimate();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}