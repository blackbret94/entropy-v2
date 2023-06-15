using UnityEngine;

namespace Vashta.Entropy.Character.Prop
{
    public class GooglyEyes : MonoBehaviour
    {
        public Animator Animator;
        public float minSpeed;
        public float maxSpeed;

        private void Start()
        {
            Animator.speed = Random.Range(minSpeed, maxSpeed);

            // Consider reversing
            if (Random.Range(0, 2) == 0)
            {
                Animator.speed *= -1;
            }
        }
    }
}