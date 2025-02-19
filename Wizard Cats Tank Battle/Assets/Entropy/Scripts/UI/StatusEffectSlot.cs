using UnityEngine;
using Vashta.Entropy.StatusEffects;
using Image = UnityEngine.UI.Image;

namespace Vashta.Entropy.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class StatusEffectSlot : GamePanel
    {
        public float BlinkThreshold = 5f;
        public float FadeOutThreshold = .5f;
        public float FadeInTime = .25f;
        
        // public Animator Animator;
        public Image Image;
        public GameObject BuffOutline;
        public GameObject DebuffOutline;
        private StatusEffectView _statusEffectView;
        private CanvasGroup _canvasGroup;
        private Vector3 _vectorOne;
        
        // DEBUG, exists for viewing in the inspector during runtime.
        [SerializeField]
        private string _debugStatusEffectName;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _vectorOne = Vector3.one;
        }

        public void SetStatusEffect(StatusEffectView statusEffectView)
        {
            // Ignore status effects that are about to fade away
            _statusEffectView = statusEffectView;

            if (statusEffectView == null || statusEffectView.StatusEffect.HasExpired())
                return;
            
            _debugStatusEffectName = statusEffectView.StatusEffect.Title();
            Image.sprite = statusEffectView.StatusEffect.Icon();
            _canvasGroup.alpha = 1;

            BuffOutline.SetActive(statusEffectView.StatusEffect.IsBuff());
            DebuffOutline.SetActive(statusEffectView.StatusEffect.IsDebuff());
        }
        
        public void ResetStatusEffect()
        {
            _statusEffectView = null;

            _debugStatusEffectName = "No effect";
            
            if(_canvasGroup)
                _canvasGroup.alpha = 0;
        }

        public StatusEffectView GetStatusEffect()
        {
            return _statusEffectView;
        }

        private void Update()
        {
            if (_statusEffectView == null || _statusEffectView.StatusEffect.HasExpired())
            {
                _canvasGroup.alpha = 0;
            }
            else
            {
                Animate();
            }
        }

        private void Animate()
        {
            if (!_canvasGroup || _statusEffectView == null)
                return;

            float timeLeft = _statusEffectView.StatusEffect.GetTimeLeft();
            float timeSinceCast = _statusEffectView.StatusEffect.TimeSinceCast();

            if (timeSinceCast < FadeInTime)
            {
                // Fading in
                float fadeInPercent = (timeSinceCast / FadeInTime);
                
                _canvasGroup.alpha = fadeInPercent;
                _canvasGroup.transform.localScale = _vectorOne * ((1+2*(1-fadeInPercent)));
            } else if (timeLeft < FadeOutThreshold)
            {
                // Fading out
                _canvasGroup.transform.localScale = _vectorOne;
                _canvasGroup.alpha = (timeLeft / FadeOutThreshold);
            } else if (timeLeft < BlinkThreshold)
            {
                // Blinking
                _canvasGroup.transform.localScale = _vectorOne;
                _canvasGroup.alpha = Mathf.PingPong(timeLeft / FadeOutThreshold,1);
            }
            else
            {
                // Solid
                _canvasGroup.transform.localScale = _vectorOne;
                _canvasGroup.alpha = 1;
            }
        }
    }
}