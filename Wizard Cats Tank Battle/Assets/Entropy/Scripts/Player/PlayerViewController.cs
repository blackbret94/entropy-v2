using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.Player
{
    public class PlayerViewController : MonoBehaviour
    {
        // UI Elements
        [Header("UI Elements")]
        [FormerlySerializedAs("label")] public Text playerNameText;
        public Slider healthSlider;
        public Slider shieldSlider;
        public Image classIcon;
        public TextMeshProUGUI HealthbarText;
        public PlayerHealthbarHUD HealthbarHUD;

        // Data sources
        [Header("Data Sources")]
        public StatusEffectDirectory StatusEffectDirectory;
        public PowerupDirectory PowerupDirectory;

        protected GameManager GameManager;

        private void Awake()
        {
            GameManager = GameManager.GetInstance();
        }
        
        public void SetName(string playerName)
        {
            playerNameText.text = playerName;
        }

        public void SetTeam(TeamDefinition teamDefinition)
        {
            playerNameText.color = teamDefinition.TeamColorPrim;
            HealthbarHUD.SetTeam(teamDefinition);
        }
        
        public void RefreshHealthSlider()
        {
            healthSlider.gameObject.SetActive(false);
            healthSlider.gameObject.SetActive(true);
        }

        public void SetHealth(int health, int maxHealth)
        {
            healthSlider.value = Mathf.Max(0f,(float)health / maxHealth);
            HealthbarText.text = $"{health} / {maxHealth}";
        }

        public void SetOvershield(int overshield, int maxOvershield)
        {
            float val = Mathf.Max(0f, (float)overshield / maxOvershield);
            
            shieldSlider.value = val;
            shieldSlider.gameObject.SetActive(val > .001f);
        }

        public void SetClassIcon(Sprite icon)
        {
            classIcon.sprite = icon;
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
    }
}