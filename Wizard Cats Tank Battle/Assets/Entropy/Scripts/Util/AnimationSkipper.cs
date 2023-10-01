using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class AnimationSkipper : MonoBehaviour
    {
        public float AnimationMaxSpeed = 100f;
        public Animator Animator;

        private void SpeedUp()
        {
            Animator.speed = AnimationMaxSpeed;
        }

        private void Update()
        {
            if (Input.GetButton("Fire1") || Input.GetButton("Fire Controller"))
            {
                SpeedUp();
            }
        }
    }
}