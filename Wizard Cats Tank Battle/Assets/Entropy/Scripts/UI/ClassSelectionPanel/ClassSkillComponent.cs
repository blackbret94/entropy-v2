using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSkillComponent : GamePanel
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public Image Icon;

        public void Set(StatusEffectData statusEffect, Color colorPrimary, Color colorSecondary)
        {
            Title.text = statusEffect.Title;
            Title.color = colorPrimary;
            
            Description.text = statusEffect.ClassDescription;
            Description.color = colorSecondary;
            
            Icon.sprite = statusEffect.EffectIcon;
        }
    }
}