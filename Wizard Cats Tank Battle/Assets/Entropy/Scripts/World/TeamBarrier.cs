using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.World
{
    public class TeamBarrier : MonoBehaviour
    {
        public int TeamId;
        public GameObject CollisionEffectAlly;
        public GameObject CollisionEffectEnemy;

        public AudioClip CollisionSfxAlly;
        public AudioClip CollisionSfxEnemy;

        // Damage to enemies is DISABLED
        private void OnTriggerEnter(Collider col)
        {
            // if it is a player, check the team
            PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if(CollisionEffectAlly)
                    PoolManager.Spawn(CollisionEffectAlly, col.transform.position, Quaternion.identity);
            
                if(CollisionSfxAlly)
                    AudioManager.Play3D(CollisionSfxAlly, col.transform.position);
            }
        }
    }
}