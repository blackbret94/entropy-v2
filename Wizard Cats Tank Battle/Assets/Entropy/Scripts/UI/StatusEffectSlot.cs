using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.UI
{
    public class StatusEffectSlot : GamePanel
    {
        public Image Image;
        private StatusEffect _statusEffect;

        public void SetStatusEffect(StatusEffect statusEffect)
        {
            _statusEffect = statusEffect;
            Image.sprite = statusEffect.Icon();
        }
    }
}