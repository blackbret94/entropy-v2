using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.UI
{
    public class HUDPanel : GamePanel
    {
        private static HUDPanel _instance;

        public static HUDPanel Get() => _instance;

        [Header("Data Sources")] 
        public StatusEffectDirectory StatusEffectDirectory;
        public PowerupDirectory PowerupDirectory;
        
        [Header("Cached references")]
        protected GameManager GameManager;

        private void Awake()
        {
            _instance = this;
            gameObject.SetActive(false);
        }
        
        private void Start()
        {
            GameManager = GameManager.GetInstance();
            // ClosePanel();
        }
        
        public void ShowPowerupIcon(int powerupSessionId)
        {
            StatusEffectData statusEffectData = StatusEffectDirectory.GetBySessionId(powerupSessionId);

            if (statusEffectData)
            {
                UIGame.GetInstance().CastPowerupButton.UpdateIcon(statusEffectData.EffectIcon);
            }
            else
            {
                Debug.LogError("Could not show powerup icon for powerup with session ID: " + powerupSessionId);
            }
        }

        public void ShowPowerupUI(int powerupId)
        {
            Powerup powerup = PowerupDirectory[powerupId];

            if (powerup == null)
                return;

            UIGame uiGame = GameManager.ui;
            uiGame.PowerUpPanel.SetText(powerup.DisplayText,powerup.DisplaySubtext, powerup.Color, powerup.Icon);
        }

        public void PlayerDied()
        {
            GameManager.ui.DropCollectiblesButton.gameObject.SetActive(false);
            GameManager.ui.CastUltimateButton.gameObject.SetActive(false);
            GameManager.ui.CastPowerupButton.gameObject.SetActive(false);
        }

        public void PlayerRespawned()
        {
            GameManager.ui.CastUltimateButton.gameObject.SetActive(true);
        }
    }
}