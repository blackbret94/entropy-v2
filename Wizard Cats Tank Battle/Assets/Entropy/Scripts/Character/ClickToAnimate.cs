using Entropy.Scripts.Audio;
using Entropy.Scripts.Player;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.Character
{
    public class ClickToAnimate : MonoBehaviour
    {
        public PlayerAnimator Animator;
        public SfxController SfxController;
        public CharacterAppearance CharacterAppearance;

        public void OnClick()
        {
            Animator.PlayRandomAnimation();
            SfxController.PlaySound(CharacterAppearance.Meow.AudioClip);
        }
    }
}