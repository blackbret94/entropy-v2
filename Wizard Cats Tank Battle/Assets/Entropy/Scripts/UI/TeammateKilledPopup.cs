using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class TeammateKilledPopup: MonoBehaviour
    {
        public Animator Animator;

        public void PlayAnimation()
        {
            Animator.SetTrigger("CoinRewarded");
        }
    }
}