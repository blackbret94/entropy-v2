using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        public Animator Animator;

        public string AttackBaseName = "Attack";
        public int AttackAnimationCount = 3;
        public string DieBaseName = "Die";
        public int DieAnimationCount = 3;
        public string TakeDamageBaseName = "Hit";
        public string HealName = "Heal";
        public int TakeDamageAnimationCount = 4;
        public string IdleBaseName = "Idle";
        public bool StartIdle = false;

        private void Start()
        {
            if(StartIdle)
                Idle();
        }
        
        public void Attack()
        {
            Animator.SetTrigger(ChooseAnimation(AttackBaseName, AttackAnimationCount));
        }

        public void Die()
        {
            Animator.SetTrigger(ChooseAnimation(DieBaseName, DieAnimationCount));
        }

        public void TakeDamage()
        {
            Animator.SetTrigger(ChooseAnimation(TakeDamageBaseName, TakeDamageAnimationCount));
        }

        public void Heal()
        {
            Animator.SetTrigger(HealName);
        }

        public void Idle()
        {
            Animator.SetTrigger(IdleBaseName);
        }

        private string ChooseAnimation(string baseName, int optionCount)
        {
            int animationChoice = Random.Range(0, optionCount) + 1;
            return baseName + animationChoice;
        }

        public void PlayRandomAnimation()
        {
            int animationIndex = Random.Range(0, 3);

            switch (animationIndex)
            {
                case 0: Attack(); break;
                case 1: Heal(); break;
                case 2: TakeDamage(); break;
            }
        }
    }
}