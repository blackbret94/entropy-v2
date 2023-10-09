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
        public string OffTrigger = "Off";

        public float BlinkThreshold = 1f;
        public float FadeOutThreshold = .334f;
        
        public Animator Animator;
        public Image Image;
        public GameObject BuffOutline;
        public GameObject DebuffOutline;
        private StatusEffect _statusEffect;
        private CanvasGroup _canvasGroup;
        
        // DEBUG
        [SerializeField]
        private string _debugStatusEffectName;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        public void SetStatusEffect(StatusEffect statusEffect)
        {
            // Ignore status effects that are about to fade away
            // if (statusEffect.GetTimeLeft() <= FadeOutThreshold + .01f)
                // return;
            
            _statusEffect = statusEffect;
            _debugStatusEffectName = statusEffect.Title();
            Image.sprite = statusEffect.Icon();

            if (statusEffect.IsFresh())
            {
                Animator.SetBool(BlinkBool, false);
                Animator.ResetTrigger(FadeOutTrigger);
                Animator.SetTrigger(FadeInTrigger);
                statusEffect.SetFresh(false);
            }
            else
            {
                Animator.SetBool(BlinkBool, false);
                Animator.ResetTrigger(FadeOutTrigger);
                _canvasGroup.alpha = 1;
            }

            BuffOutline.SetActive(statusEffect.IsBuff());
            DebuffOutline.SetActive(statusEffect.IsDebuff());
        }
        
        public void ResetStatusEffect()
        {
            Animator.SetBool(BlinkBool, false);
            Animator.ResetTrigger(FadeInTrigger);
            Animator.ResetTrigger(FadeOutTrigger);
            Animator.SetTrigger(OffTrigger);
            _statusEffect = null;

            _debugStatusEffectName = "No effect";
            // Image.sprite = null;
            _canvasGroup.alpha = 0;
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
            else
            {
                if (Animator.GetBool(BlinkBool))
                {
                    Animator.SetBool(BlinkBool, false);
                    Animator.SetTrigger(FadeInTrigger);
                }
            }
        }

        public void Close()
        {
            Animator.SetBool(BlinkBool, false);
            Animator.SetTrigger(FadeOutTrigger);
        }
        
        public void AnimationEnd()
        {
            ResetStatusEffect();
        }
    }
}