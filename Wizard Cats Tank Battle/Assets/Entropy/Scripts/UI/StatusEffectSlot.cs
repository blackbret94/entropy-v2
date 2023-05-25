using System;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class StatusEffectSlot : GamePanel
    {
        public string FadeInTrigger = "Fade In";
        public string BlinkBool = "Blinking";
        public string FadeOutTrigger = "Fade Out";

        public float BlinkThreshold = 1f;
        public float FadeOutThreshold = .334f;
        
        public Animator Animator;
        public Image Image;
        private StatusEffect _statusEffect;
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        public void SetStatusEffect(StatusEffect statusEffect)
        {
            // Ignore status effects that are about to fade away
            if (statusEffect.GetTimeLeft() <= FadeOutThreshold + .01f)
                return;
            
            _statusEffect = statusEffect;
            Image.sprite = statusEffect.Icon();
            Animator.SetBool(BlinkBool, false);
            Animator.SetTrigger(FadeInTrigger);
        }

        public StatusEffect GetStatusEffect()
        {
            return _statusEffect;
        }

        private void Update()
        {
            if(_statusEffect != null)
                UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            float timeLeft = _statusEffect.GetTimeLeft();

            if (timeLeft < FadeOutThreshold)
            {
                Close();
            } else if (timeLeft < BlinkThreshold)
            {
                Animator.SetBool(BlinkBool, true);
            }
        }

        public void Close()
        {
            Animator.SetBool(BlinkBool, false);
            Animator.SetTrigger(FadeOutTrigger);
            _statusEffect = null;
        }
    }
}