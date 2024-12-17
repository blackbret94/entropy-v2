using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Spells;

namespace Entropy.Scripts.Player
{
    public class UltimateController : MonoBehaviour
    {
        [Header("Cached references")] 
        private TanksMP.Player _player;
        private PhotonView _view;

        private void Awake()
        {
            _player = GetComponent<TanksMP.Player>();
            _view = GetComponent<PhotonView>();
        }
        
        // Server only
        public void IncreaseUltimate()
        {
            if(!PhotonNetwork.IsMasterClient)
                return;

            // Only give to living players
            if (!_player.IsAlive)
                return;

            int ultimateCost = _player.GetClass().ultimateCost;
            if(_view.GetUltimate() < ultimateCost)
                _view.SetUltimate(_view.GetUltimate()+1);
        }

        // Server only
        public void RewardUltimateForKill()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            // Only give to living players
            if (!_player.IsAlive)
                return;

            int ultimateIncrease = 5;
            int ultimateCost = _player.GetClass().ultimateCost;
            int currentUltimate = _view.GetUltimate();

            if (currentUltimate < ultimateCost)
            {
                _view.SetUltimate(currentUltimate + ultimateIncrease);
            }
        }

        // Server only
        public void ClearUltimate()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            _view.SetUltimate(0);
        }
        
        public int GetUltimate()
        {
            return _view.GetUltimate();
        }

        public float GetUltimatePerun()
        {
            int ultimateCost = _player.GetClass().ultimateCost;
            return (float)_view.GetUltimate() / ultimateCost;
        }

        /// <summary>
        /// Called by local player
        /// </summary>
        /// <returns></returns>
        public bool TryCastUltimate()
        {
            int ultimateCost = _player.GetClass().ultimateCost;
            
            if (_view.GetUltimate() >= ultimateCost)
            {
                _view.RPC("RpcClearUltimate", RpcTarget.MasterClient);
                _view.RPC("RpcCastUltimate", RpcTarget.All);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}