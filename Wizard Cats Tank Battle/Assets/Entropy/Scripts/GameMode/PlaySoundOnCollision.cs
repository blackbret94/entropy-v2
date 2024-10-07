using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameMode
{
    public class PlaySoundOnCollision : MonoBehaviour
    {
        public GameObject CollisionEffect;
        public AudioClip CollisionSfx;

        private void OnTriggerEnter(Collider col)
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player != null)
            {
                if (CollisionEffect)
                    PoolManager.Spawn(CollisionEffect, col.transform.position, Quaternion.identity);

                if (CollisionSfx)
                    AudioManager.Play3D(CollisionSfx, col.transform.position);
            }
        }
    }
}