using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class TriggerAnimationOnce : MonoBehaviour
    {
        public Animator Animator;
        public string Trigger = "Start";

        private void Start()
        {
            Animator.SetTrigger(Trigger);
        }
    }
}