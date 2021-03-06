using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using TanksMP;

namespace Entropy.Scripts.Player
{
    public class PlayerCollisionHandler: MonoBehaviour
    {
        public TanksMP.Player player;
        
        /// <summary>
        /// the amount of damage done by this tank when it hits another tank
        /// </summary>
        public int damageAmtOnCollision = 5;
        
        /// <summary>
        /// Mitigates collision damage
        /// </summary>
        public int armor = 2;
        
        /// <summary>
        /// Object to spawn when it collides with another player
        /// </summary>
        public GameObject collisionFx;

        /// <summary>
        /// Audio to play on collision
        /// </summary>
        public AudioClip collisionClip;
        
        private List<PlayerCollisionHandler> _playersActivelyCollided;

        private void Start()
        {
            _playersActivelyCollided = new List<PlayerCollisionHandler>();
        }

        private void OnCollisionEnter(Collision col)
        {
            PlayerCollisionHandler colPlayer = GetPlayerFromCollision(col);

            if (colPlayer == null)
                return;


            if (_playersActivelyCollided.Contains(colPlayer))
                return;
            
            _playersActivelyCollided.Add(colPlayer);
            PlayCollisionFx(col.contacts[0].point);

            HandleCollisionServer(colPlayer);
        }

        private void OnCollisionExit(Collision col)
        {
            PlayerCollisionHandler colPlayer = GetPlayerFromCollision(col);

            if (colPlayer == null)
                return;

            if (_playersActivelyCollided.Contains(colPlayer))
                _playersActivelyCollided.Remove(colPlayer);
        }

        private PlayerCollisionHandler GetPlayerFromCollision(Collision col)
        {
            GameObject obj = col.gameObject;
            return obj.GetComponent<PlayerCollisionHandler>();
        }

        private void HandleCollisionServer(PlayerCollisionHandler colPlayer)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            TanksMP.Player otherPlayer = colPlayer.GetComponent<TanksMP.Player>();

            player.TakeDamage(CalculateDamage(colPlayer), otherPlayer);
        }

        private int CalculateDamage(PlayerCollisionHandler colPlayer)
        {
            if (!colPlayer)
            {
                Debug.LogError("Player is missing a PlayerCollisionHandler!");
                return 0;
            }
            
            int damage = damageAmtOnCollision;
            int otherArmor = colPlayer.armor;

            return Mathf.Max(0, damage - otherArmor);
        }

        private void PlayCollisionFx(Vector3 position)
        {
            if (collisionClip) 
                AudioManager.Play3D(collisionClip, transform.position);
            
            if (collisionFx)
                PoolManager.Spawn(collisionFx, position, transform.rotation);
        }
    }
}