using Entropy.Scripts.Player;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class CastPowerupButton : GamePanel
    {
        public Image PowerupIcon;

        private Player _localPlayer;

        private void Start()
        {
            FindLocalPlayer();
            ClosePanel();
        }
        
        private void FindLocalPlayer()
        {
            _localPlayer = PlayerList.GetLocalPlayer();
        }

        public void UpdateIcon(Sprite sprite)
        {
            PowerupIcon.sprite = sprite;
            OpenPanel();
        }

        public void CastPowerup()
        {
            if(!_localPlayer)
                FindLocalPlayer();

            if (!_localPlayer)
            {
                Debug.LogError("Could not find local player!");
                return;
            }
            
            _localPlayer.TryCastPowerup();
        }

        public void ResetPowerup()
        {
            ClosePanel();
        }
    }
}