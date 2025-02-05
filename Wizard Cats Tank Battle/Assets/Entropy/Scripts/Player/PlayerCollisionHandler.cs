using UnityEngine;
using System.Collections.Generic;
using TanksMP;
using UnityEngine.Serialization;
using Vashta.Entropy.Player;

namespace Entropy.Scripts.Player
{
    public class PlayerCollisionHandler: MonoBehaviour
    {
        [FormerlySerializedAs("player")] public PlayerController playerController;
        
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

            _hasInit = true;
        }

        private void OnCollisionEnter(Collision col)
        {
            Init();
            
            PlayerCollisionHandler colPlayer = GetPlayerFromCollision(col);

            if (colPlayer == null)
                return;

            // Ignore if player does not have spikes
            // bool playerHasSpikes = player.StatusEffectController.SpikeDamageModifier > 0;
            bool otherPlayerHasSpikes = colPlayer.playerController.StatusEffectController.SpikeDamageModifier > 0;
            if (!otherPlayerHasSpikes)
                return;
            
            // ignore active collisions
            if (_playersActivelyCollided.Contains(colPlayer))
                return;

            // ignore team mates
            if (playerController.TeamIndex == colPlayer.playerController.TeamIndex)
                return;
            
            _playersActivelyCollided.Add(colPlayer);
            PlayCollisionFx(col.contacts[0].point);

            PlayerController otherPlayerController = colPlayer.GetComponent<PlayerController>();

            playerController.CombatController.TakeDamage(CalculateDamage(colPlayer, otherPlayerController), otherPlayerController);
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
        
        private int CalculateDamage(PlayerCollisionHandler colPlayer, PlayerController otherPlayerController)
        {
            if (!colPlayer)
            {
                Debug.LogError("Player is missing a PlayerCollisionHandler!");
                return 0;
            }

            int damage = damageAmtOnCollision + Mathf.RoundToInt(otherPlayerController.StatusEffectController.SpikeDamageModifier);
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