using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class CoinsEarnedPopup: MonoBehaviour
    {
        public Animator Animator;
        public TextMeshProUGUI AnimationText;

        public void PlayAnimation(int value)
        {
            AnimationText.text = "+"+value;
            Animator.SetTrigger("CoinRewarded");
        }
    }
}