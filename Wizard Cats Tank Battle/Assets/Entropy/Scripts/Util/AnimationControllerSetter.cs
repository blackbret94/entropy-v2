using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class AnimationControllerSetter : MonoBehaviour
    {
        public Animator Animator;
        public string TriggerToSet;

        private void Start()
        {
            Animator.SetTrigger(TriggerToSet);
        }
    }
}