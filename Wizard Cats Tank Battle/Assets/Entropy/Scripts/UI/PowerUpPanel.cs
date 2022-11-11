using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class PowerUpPanel : GamePanel
    {
        public TextMeshProUGUI TextTitle;
        public TextMeshProUGUI TextSubtitle;
        public Animator Animator;

        public void SetText(string title, string subtitle, Color color)
        {
            gameObject.SetActive(true);
            TextTitle.text = title;
            TextSubtitle.text = subtitle;
            TextSubtitle.color = color;
            Animator.SetTrigger("PowerUpEarned");
        }
    }
}