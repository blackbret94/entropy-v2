using TanksMP;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class CastUltimateButton : MonoBehaviour
    {
        public Image SpellIcon;
        public Slider UltimateSlider;
        
        private Player _localPlayer;
        
        private const float LERP_RATE = 20f;
        private const float LERP_CLAMP = .1f;
        private float _currentDisplayedValue = 0f;
        private bool isReady = false;
        
        private void Start()
        {
            FindLocalPlayer();
            _currentDisplayedValue = 0f;
            UltimateSlider.value = 0f;
        }
        
        public void CastUltimateLocalPlayer()
        {
            if(!_localPlayer)
                FindLocalPlayer();

            if (!_localPlayer)
            {
                Debug.LogError("Could not find local player!");
                return;
            }
            
            bool couldCast = _localPlayer.TryCastUltimate();

            if (!couldCast)
            {
                GameManager.GetInstance().ui.SfxController.PlayUltimateNotReady();
            }
        }

        public void UpdateSpellIcon(Sprite sprite)
        {
            SpellIcon.sprite = sprite;
        }

        public void ResetUltimate()
        {
            _currentDisplayedValue = 0f;
            UltimateSlider.value = 0;
            isReady = false;
        }

        private void Update()
        {
            if(!_localPlayer)
                FindLocalPlayer();
            
            if (!_localPlayer)
            {
                Debug.LogError("Could not find local player!");
                return;
            }

            // Get perun and INVERT
            float newUltimateValue = 1 - _localPlayer.GetUltimatePerun();

            // Play sfx
            if (newUltimateValue > .01)
            {
                isReady = false;
            }
            else
            {
                if(!isReady)
                    UIGame.GetInstance().SfxController.PlayUltimateReady();
                
                isReady = true;
            }

            // Clamp
            if (newUltimateValue > .99 || newUltimateValue < .01)
            {
                _currentDisplayedValue = newUltimateValue;
            }
            else if (Mathf.Abs(_currentDisplayedValue - newUltimateValue) < LERP_CLAMP)
            {
                _currentDisplayedValue = newUltimateValue;
            }
            else
            {
                // Lerp
                _currentDisplayedValue = Mathf.Lerp(_currentDisplayedValue, newUltimateValue, LERP_RATE * Time.deltaTime);
            }
            
            // Apply
            UltimateSlider.value = _currentDisplayedValue;
        }

        private void FindLocalPlayer()
        {
            _localPlayer = Player.GetLocalPlayer();
        }
    }
}