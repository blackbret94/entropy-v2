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
        
        private void Start()
        {
            FindLocalPlayer();
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
            
            _localPlayer.TryCastUltimate();
        }

        public void UpdateSpellIcon(Sprite sprite)
        {
            SpellIcon.sprite = sprite;
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
            
            UltimateSlider.value = 1-_localPlayer.GetUltimatePerun();
        }

        private void FindLocalPlayer()
        {
            _localPlayer = Player.GetLocalPlayer();
        }
    }
}