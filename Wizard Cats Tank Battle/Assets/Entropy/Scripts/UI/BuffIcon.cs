using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class BuffIcon : GamePanel
    {
        public PowerupDirectory BuffDirectory;
        public Image Icon;
        public Slider BuffTimeSlider;
        private float _maxSeconds = 30f; // fix the source of this

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void SetPowerup(int buffId, float timeSeconds)
        {
            if (timeSeconds <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
                
            
            if (buffId > 0)
            {
                gameObject.SetActive(true);
                Powerup powerup = BuffDirectory[buffId];
                Icon.sprite = powerup.Icon;
                Icon.color = powerup.Color;
                _maxSeconds = powerup.MaxValue;
            }

            if (buffId == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                SetSeconds(timeSeconds);
            }
        }

        public void SetSeconds(float seconds)
        {
            if(seconds <= 0f)
                gameObject.SetActive(false);
            
            BuffTimeSlider.value = (seconds / _maxSeconds);
        }
    }
}