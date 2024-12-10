using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI.GameLog
{
    public class GameLogRowPanel : GamePanel
    {
        public Animator Animator;
        public string FadeInAnimationKey;
        public string FadeOutAnimationKey;
        // public string ResetAnimationKey;
        public TextMeshProUGUI Text;

        public void SetText(GameLogRow gameLogRow)
        {
            Text.text = gameLogRow.Text;
            if (gameLogRow.IsFresh)
            {
                if(Animator != null)
                    Animator.SetTrigger(FadeInAnimationKey);
                
                gameLogRow.IsFresh = false;
            }
            else
            {
                // Animator.SetTrigger(ResetAnimationKey);
            }
        }

        public void ResetText()
        {
            Text.text = string.Empty;
        }

        public void FadeOut()
        {
            Animator.SetTrigger(FadeOutAnimationKey);
        }
    }
}