using EckTechGames.FloatingCombatText;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vashta.Entropy.Character;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.Player
{
    public class PlayerViewController : MonoBehaviour
    {
        [Header("UI Elements")]
        [FormerlySerializedAs("label")] public Text playerNameText;
        public Slider healthSlider;
        public Slider shieldSlider;
        public Image classIcon;
        public TextMeshProUGUI HealthbarText;
        public PlayerHealthbarHUD HealthbarHUD;
        public PlayerAimGraphic PlayerAimGraphic;

        [Header("Controllers")] 
        private PlayerController _playerController;
        private PlayerAnimator _playerAnimator;
        private StatusEffectController _statusEffectController;
        private CharacterAppearance _characterAppearance;
        
        [Header("Data Sources")]
        public VisualEffectDirectory VisualEffectDirectory;

        [Header("Cached references")]
        protected GameManager GameManager;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _statusEffectController = GetComponent<StatusEffectController>();
            _characterAppearance = _playerController.CharacterAppearance;
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

        public void ShowDamageText(int damage, bool attackerIsCounter, bool attackerIsSame)
        {
            if (damage == 0)
                return;
            
            // Show damage
            if (damage > 0)
            {
                if (attackerIsCounter)
                {
                    OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.CriticalHit,
                        damage);
                }
                else if (attackerIsSame)
                {
                    OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.Miss, damage);
                }
                else
                {
                    OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.Hit, damage);
                }
            }
            else
            {
                // Show heals
                OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.Heal, Mathf.Abs(damage));
            }

            // animate
            if (damage > 0)
                _playerAnimator.TakeDamage();
            else
                _playerAnimator.Heal();
        }
        
        public void SpawnDeathFx(ushort killingBlowDeathFx)
        {
            VisualEffectData deathFxData;
            if (killingBlowDeathFx == 0)
                deathFxData = VisualEffectDirectory.GetBySessionId(_statusEffectController.GetDeathFx());
            else
                deathFxData = VisualEffectDirectory.GetBySessionId(killingBlowDeathFx);

            if (!deathFxData)
            {
                Debug.LogWarning("Could not load death fx with ID: " + killingBlowDeathFx);
                deathFxData = _playerController.GetClass().DefaultDeathEffects();
            }
            
            if(deathFxData != null)
                PoolManager.Spawn(deathFxData.VisualEffectPrefab, transform.position, transform.rotation);
        }
        
        public void ColorizePlayerForTeam(TeamInstance teamInstance = null)
        {
            if (teamInstance == null && _playerController.TeamIndex > -1)
            {
                teamInstance = GameManager.TeamController.teams[_playerController.TeamIndex];
            }

            if (teamInstance == null)
                return;

            //get corresponding team and colorize renderers in team color
            _characterAppearance.teamInstance = teamInstance;
            _characterAppearance.ColorizeCart();
            
            SetTeam(teamInstance.teamDefinition);   

            if (_playerController.IsLocal)
            {
                if (PlayerAimGraphic)
                {
                    PlayerAimGraphic.SetColor(teamInstance.teamDefinition.GetPrimaryColorLight());
                }
            }
            else
            {
                if (PlayerAimGraphic)
                {
                    PlayerAimGraphic.Disable();
                }
            }
        }
        
        public void RewardCoins(int amount)
        {
            OverlayCanvasController.instance.ShowCombatText(gameObject, CombatTextType.CoinReward, "+"+amount);

            // play coin reward sound
            GameManager.ui.SfxController.PlayCoinEarnedSound();
        }
    }
}