using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.SaveLoad
{
    public class TeamBarrier : MonoBehaviour
    {
        public int TeamId;
        public GameObject CollisionEffectAlly;
        public GameObject CollisionEffectEnemy;

        public AudioClip CollisionSfxAlly;
        public AudioClip CollisionSfxEnemy;

        private void OnTriggerEnter(Collider col)
        {
            // if it is a player, check the team
            Player player = col.gameObject.GetComponent<Player>();
            if (player != null)
            {
                if (player.GetView().GetTeam() == TeamId)
                {
                    // Ally
                    SpawnCollisionEffectAlly();
                }
                else
                {
                    // Enemy
                    SpawnCollisionEffectEnemy();
                    player.CmdKillPlayer();
                }
            }
        }

        private void SpawnCollisionEffectAlly()
        {
            if(CollisionEffectAlly)
                PoolManager.Spawn(CollisionEffectAlly, transform.position, Quaternion.identity);
            
            if(CollisionSfxAlly)
                AudioManager.Play3D(CollisionSfxAlly, transform.position);
        }

        private void SpawnCollisionEffectEnemy()
        {
            if(CollisionEffectEnemy)
                PoolManager.Spawn(CollisionEffectEnemy, transform.position, Quaternion.identity);
            
            if(CollisionSfxEnemy)
                AudioManager.Play3D(CollisionSfxEnemy, transform.position);
        }
    }
}