using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class PowerUpPanel : GamePanel
    {
        public TextMeshProUGUI TextTitle;
        public TextMeshProUGUI TextSubtitle;
        public Image IconImage;
        public Animator Animator;

        public void SetText(string title, string subtitle, Color color, Sprite sprite)
        {
            gameObject.SetActive(true);
            TextTitle.text = title;
            TextSubtitle.text = subtitle;
            TextSubtitle.color = color;
            IconImage.sprite = sprite;
            Animator.SetTrigger("PowerUpEarned");
        }
    }
}