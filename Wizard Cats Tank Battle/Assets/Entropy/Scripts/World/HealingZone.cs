using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.World
{
    public class HealingZone : MonoBehaviour
    {
        private List<PlayerController> _playersInZone;
        
        public int HealTeamId = 0;
        public int HealAmount = 2;
        public float HealRateS = 1f;

        private float _lastHeal = 0f;
        private Collider _collider;
        
        private void Start()
        {
            _playersInZone = new List<PlayerController>();
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (!_collider)
            {
                Debug.LogError("Missing collider connection!");
                return;
            }
            
            if (_lastHeal + HealRateS <= Time.time)
            {
                // Allow safe mutation of collection
                List<PlayerController> playersInZoneSafeCopy = new List<PlayerController>(_playersInZone);
                
                foreach (PlayerController player in playersInZoneSafeCopy)
                {
                    if (player == null || !player.IsAlive || player.TeamIndex != HealTeamId || !_collider.bounds.Contains(player.transform.position))
                        _playersInZone.Remove(player);
                    else
                        player.Heal(HealAmount);
                }
                
                _lastHeal = Time.time;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null && playerController.TeamIndex == HealTeamId)
            {
                _playersInZone.Add(playerController);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            _playersInZone.Remove(playerController);
        }
    }
}