using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.World
{
    public class HealingZone : MonoBehaviour
    {
        private List<Player> _playersInZone;
        
        public int HealTeamId = 0;
        public int HealAmount = 2;
        public float HealRateS = 1f;

        private float _lastHeal = 0f;
        private Collider _collider;
        
        private void Start()
        {
            _playersInZone = new List<Player>();
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            // only execute on the server
            if (!PhotonNetwork.IsMasterClient)
                return;

            if (!_collider)
            {
                Debug.LogError("Missing collider connection!");
                return;
            }
            
            if (_lastHeal + HealRateS <= Time.time)
            {
                // Allow safe mutation of collection
                List<Player> playersInZoneSafeCopy = new List<Player>(_playersInZone);
                
                foreach (Player player in playersInZoneSafeCopy)
                {
                    if (player == null || !player.IsAlive || player.GetView().GetTeam() != HealTeamId || !_collider.bounds.Contains(player.transform.position))
                        _playersInZone.Remove(player);
                    else
                        player.Heal(HealAmount);
                }
                
                _lastHeal = Time.time;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null && player.GetView().GetTeam() == HealTeamId)
            {
                _playersInZone.Add(player);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            Player player = other.gameObject.GetComponent<Player>();
            _playersInZone.Remove(player);
        }
    }
}