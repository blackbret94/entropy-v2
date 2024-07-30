using System.Collections;
using TanksMP;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class MatchTimerPanel : GamePanel
    {
        public Animator TextAnimator;
        public string AnimationName = "Animation";
        public TextMeshProUGUI Textbox;
        
        private float _timerRefreshRate = .25f;
        private float _lastUpdateTime;
        private int _lastSavedMatchTime;
        private void Update()
        {
            // slow update
            if (Time.time > _lastUpdateTime + _timerRefreshRate)
            {
                UpdateTimer();
                _lastUpdateTime = Time.time;
            }
        }
        
        private void UpdateTimer()
        {
            int currentMatchTime = GameManager.GetInstance().MatchTimer.CurrentMatchTime();
            // play animation?
            if (currentMatchTime != _lastSavedMatchTime && currentMatchTime <= 10)
            {
                if (TextAnimator)
                {
                    TextAnimator.Play(AnimationName);
                }
            }

            string minutes = (currentMatchTime / 60).ToString("00");
            string seconds = (currentMatchTime % 60).ToString("00");
            Textbox.text = $"{minutes}:{seconds}";
            
            _lastSavedMatchTime = currentMatchTime;
        }
    }
}