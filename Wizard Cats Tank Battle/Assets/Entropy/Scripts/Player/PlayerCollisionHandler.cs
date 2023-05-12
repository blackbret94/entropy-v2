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

        private bool _hasInit = false;
        
        private List<PlayerCollisionHandler> _playersActivelyCollided;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_hasInit)
                return;
            
            _playersActivelyCollided = new List<PlayerCollisionHandler>();
            player.PlayerCollisionHandler = this;

            _hasInit = true;
        }

        private void OnCollisionEnter(Collision col)
        {
            Init();
            
            PlayerCollisionHandler colPlayer = GetPlayerFromCollision(col);

            if (colPlayer == null)
                return;

            // ignore active collisions
            if (_playersActivelyCollided.Contains(colPlayer))
                return;

            // ignore team mates
            if (player.GetView().GetTeam() == colPlayer.player.GetView().GetTeam())
                return;
            
            // ignore players that have spike buffs
            if (player.StatusEffectController.SpikeDamageModifier > 0)
                return;
            
            _playersActivelyCollided.Add(colPlayer);
            PlayCollisionFx(col.contacts[0].point);

            HandleCollisionServer(colPlayer);
        }

        private void OnCollisionExit(Collision col)
        {
            Init();
            
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

            player.TakeDamage(CalculateDamage(colPlayer, otherPlayer), otherPlayer);
        }

        private int CalculateDamage(PlayerCollisionHandler colPlayer, TanksMP.Player otherPlayer)
        {
            if (!colPlayer)
            {
                Debug.LogError("Player is missing a PlayerCollisionHandler!");
                return 0;
            }

            int damage = damageAmtOnCollision + Mathf.RoundToInt(otherPlayer.StatusEffectController.SpikeDamageModifier);
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