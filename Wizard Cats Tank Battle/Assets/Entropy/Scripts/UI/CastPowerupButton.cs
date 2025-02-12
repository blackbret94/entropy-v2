using Entropy.Scripts.Player;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.UI
{
    public class CastPowerupButton : GamePanel
    {
        public Image PowerupIcon;

        private PlayerController _localPlayerController;

        private void Start()
        {
            FindLocalPlayer();
            ClosePanel();
        }
        
        private void FindLocalPlayer()
        {
            _localPlayerController = PlayerList.GetLocalPlayer();
        }

        public void UpdateIcon(Sprite sprite)
        {
            PowerupIcon.sprite = sprite;
            OpenPanel();
        }

        public void CastPowerup()
        {
            if(!_localPlayerController)
                FindLocalPlayer();

            if (!_localPlayerController)
            {
                Debug.LogError("Could not find local player!");
                return;
            }
            
            _localPlayerController.PowerupController.TryCastPowerup();
        }

        public void ResetPowerup()
        {
            ClosePanel();
        }
    }
}