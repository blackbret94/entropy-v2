using System.Collections.Generic;
using System.Linq;
using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.Player;
using Vashta.Entropy.UI;

namespace Vashta.Entropy.StatusEffects
{
    public class StatusEffectController : NetworkBehaviour
    {
        [Header("Data Sources")]
        public StatusEffectDirectory StatusEffectDirectory;
        public StatusEffectPanel StatusEffectPanel;
        
        [Header("Cached references")]
        private PlayerController _playerController;
        private PlayerStatusEffectVisualizer _visualizer;

        [Networked, Capacity(10), OnChangedRender(nameof(OnStatusEffectsChange))] 
        private NetworkDictionary<ushort, StatusEffectNetwork> _statusEffects => default;
        private bool _dirtyFlag;
        
        private const float _refreshRateS = .5f;
        private float _lastRefresh = 0;
        private PlayerController _lastDotAppliedBy;
        
        // effects
        private float _massMultiplierCashed = 1f;
        private float _movementSpeedModifierCached = 0f;
        private float _movementSpeedMultiplierCached = 1f;
        private float _damageOutputModifierCached = 1f;
        private float _damageTakenModifierCached = 0f;
        private float _healthPerSecondCached = 0f;
        private int _leechingPerSecondCached = 0;
        private float _attackRateModifierCached = 1f;
        private float _spikeDamageModifierCached = 0f;
        private bool _isReflectiveCached = false;
        private bool _disableFiringCached = false;
        private bool _projectileExplodesCached = false;
        private bool _projectileReflectsCached = false;
        private float _projectileLifeExtendedCached = 0f;
        private int _additionalProjectilesSprayCached = 0;
        private bool _piercesCached = false;
        
        // behavior changes
        private bool _blocksBuffsCached = false;
        private bool _blocksDebuffsCached = false;
        private bool _blocksCastingBuffsCached = false;
        private bool _blocksCastingDebuffsCached = false;
        private PlayerController _leechingAppliedByCached;
        private bool _buffsLastForeverCached = false;

        private StatusEffectData _bloodVengeanceChainedEffect;

        public float MassMultiplier => _massMultiplierCashed;
        public float MovementSpeedModifier => _movementSpeedModifierCached;
        public float MovementSpeedMultiplier => _movementSpeedMultiplierCached;
        public float DamageOutputModifier => _damageOutputModifierCached;
        public float DamageTakenModifier => _damageTakenModifierCached;
        public float HealthPerSecond => _healthPerSecondCached;
        public int LeechingPerSecond => _leechingPerSecondCached;
        public bool DisableFiring => _disableFiringCached;
        public float AttackRateModifier => _attackRateModifierCached;
        public float SpikeDamageModifier => _spikeDamageModifierCached;
        public bool IsReflective => _isReflectiveCached;
        public bool BlocksBuffs => _blocksBuffsCached;
        public bool BlocksDebuffs => _blocksDebuffsCached;
        public bool BuffsLastForever => _buffsLastForeverCached;
        public bool BlocksCastingBuffs => _blocksCastingBuffsCached;
        public bool BlocksCastingDebuffs => _blocksCastingDebuffsCached;
        public PlayerController LeechingAppliedBy => _leechingAppliedByCached;

        public bool ProjectileExplodes => _projectileExplodesCached;
        public bool ProjectileReflects => _projectileReflectsCached;
        public float ProjectileLifeExtended => _projectileLifeExtendedCached;
        public int AdditionalProjectilesSpray => _additionalProjectilesSprayCached;
        public bool Pierces => _piercesCached;
        
        
        public PlayerController LastDotAppliedBy => _lastDotAppliedBy;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _visualizer = GetComponent<PlayerStatusEffectVisualizer>();
        }

        public List<StatusEffectView> GetStatusEffectViews()
        {
            List<StatusEffectView> statusEffectViews = new List<StatusEffectView>();
            
            foreach (KeyValuePair<ushort, StatusEffectNetwork> statusEffectNetwork in _statusEffects)
            {
                StatusEffectNetwork statusEffect = statusEffectNetwork.Value;

                if (statusEffect.IsValid())
                {
                    statusEffectViews.Add(statusEffect.GetView());
                }
            }

            return statusEffectViews;
        }
        
        public virtual void StatusEffectTick()
        {
            // leech
            Leech();
            
            // handle health changes from DoTs/HoTs
            int health = _playerController.Health;
            int healthPerSecond = Mathf.RoundToInt(HealthPerSecond);
            
            if (healthPerSecond != 0)
            {
                int shield = _playerController.Shield;
                if (shield > 0 && healthPerSecond < 0)
                {
                    _playerController.SetShield(shield-1);
                }
                else
                {
                    health += healthPerSecond;
                    _playerController.SetHealth(health);
                }
            }
            
            if(healthPerSecond != 0 && (healthPerSecond < 0 || health < _playerController.maxHealth))
                _playerController.PlayerViewController.ShowDamageText(-healthPerSecond, false, false);

            if (health <= 0 && HasStateAuthority)
            {
                ushort deathFx = GetDeathFx();
                
                // killed the player
                _playerController.CombatController.KillPlayer(LastDotAppliedBy, deathFx);
            }
            
            // Bot specific logic
            PlayerControllerBot playerControllerBot = _playerController as PlayerControllerBot;

            if (playerControllerBot != null)
            {
                // adjust speed
                float speed = ((playerControllerBot.moveSpeed + MovementSpeedModifier) *
                               MovementSpeedMultiplier);
                playerControllerBot.agent.speed = speed;
            }
        }
        
        /// <summary>
        /// Server only, call RPC on all clients
        /// </summary>
        /// <param name="statusEffectId"></param>
        /// <param name="owner"></param>
        public void AddStatusEffect(string statusEffectId, PlayerController owner)
        {
            StatusEffectData statusEffectData = StatusEffectDirectory[statusEffectId];
            if (statusEffectData == null)
            {
                Debug.LogError("Invalid status effect UUID: " + statusEffectId);
                return;
            }

            ushort statusEffectSessionId = statusEffectData.SessionId;
            
            // Could eventually optimize this so new structs are not created if it is already in the dictionary.
            StatusEffectNetwork statusEffect = new StatusEffectNetwork(statusEffectSessionId, owner.PlayerId, Runner.SimulationTime);
            
            // Check if status effect already exists
            StatusEffectNetwork existingEffect = StatusEffectAlreadyExists(statusEffectSessionId);
            
            // If a buff and buffs are blocked, return
            if (!statusEffect.IsImmuneToRemoval() && (statusEffect.IsBuff() && _blocksBuffsCached))
                return;
            
            // If a debuff and debuffs are blocked, return
            if (!statusEffect.IsImmuneToRemoval() && (statusEffect.IsDebuff() && _blocksDebuffsCached))
                return;
            
            // Clear buffs if this blocks buffs
            if(statusEffect.BlocksBuffs())
                ClearBuffs();
            
            // Clear debuffs if this blocks debuffs
            if (statusEffect.BlocksDebuffs())
                ClearDebuffs();
            
            // Apply fx
            _visualizer.AddEffect(statusEffectData.SessionId, statusEffect.ApplyFxData());

            // Alert if local player
            if (_playerController.IsLocal && !existingEffect.IsValid())
            {
                // Show panel
                GameManager.GetInstance().ui.PowerUpPanel.SetText(statusEffect.Title(), statusEffect.Description(),
                    statusEffect.Color(), statusEffect.Icon());

                // Play fx
                if(statusEffect.Sfx())
                    AudioManager.Play2D(statusEffect.Sfx());
            }
            
            // Apply effects instantly if configured to do so
            if (statusEffect.ApplyInstantly())
            {
                int healthPerSecond = Mathf.RoundToInt(statusEffect.HealthPerSecond());

                if (healthPerSecond > 0)
                {
                    _playerController.Heal(healthPerSecond);
                }
                else if (healthPerSecond < 0)
                {
                    _playerController.CombatController.TakeDamage(healthPerSecond, statusEffect.OriginPlayer());
                }

                // Do NOT add as status effect if it is supposed to be instantly applied
                return;
            }
            
            if (!existingEffect.IsValid())
            {
                // If it doesn't exist, add it
                _statusEffects.Add(statusEffectSessionId, statusEffect);
                _dirtyFlag = true;
            }
            else
            {
                // If it exists, update TTL
                existingEffect.SetExpiration(Runner.SimulationTime);
                _statusEffects.Set(statusEffectSessionId, existingEffect);
                // existingEffect.SetFresh(true);
            }
            
            // Refresh the panel
            StatusEffectPanel.ForceRefresh();
        }

        public void ClearStatusEffects()
        {
            _statusEffects.Clear();
            _dirtyFlag = true;
            StatusEffectPanel.ResetSlots();
            _visualizer.Clear();
        }

        public void OnStatusEffectsChange()
        {
            
            
            _dirtyFlag = true;
            //
            // string effects = default;
            // foreach (KeyValuePair<ushort,StatusEffectNetwork> statusEffect in _statusEffects)
            // {
            //     effects += statusEffect.Value.Title() + " ";
            // }
            //
            // Debug.Log(effects);
        }
        
        
        public override void FixedUpdateNetwork(){}
        
        public override void Render()
        {
            if(Runner.SimulationTime + _refreshRateS >= _lastRefresh)
                CheckLifeOfStatusEffects();
            
            if(_dirtyFlag)
                RefreshCache();
        }
        
        private void CheckLifeOfStatusEffects()
        {
            // Duplicate to safely iterate
            NetworkDictionary<ushort, StatusEffectNetwork> statusEffects = _statusEffects;

            foreach (KeyValuePair<ushort, StatusEffectNetwork> statusEffect in statusEffects)
            {
                StatusEffectNetwork statusEffectNetwork = statusEffect.Value;
                
                // Refresh expiration if buffs last forever
                // Makes sure that this is NOT a buffsLastForever effect, as that would be preserved forever
                if (_buffsLastForeverCached && !statusEffectNetwork.BuffsLastForever())
                {
                    statusEffects[statusEffect.Key].SetExpiration(Runner.SimulationTime);
                }
                
                if (statusEffectNetwork.HasExpired())
                {
                    RemoveStatusEffect(statusEffectNetwork);
                }
            }
            
            _lastRefresh = Runner.SimulationTime;
        }
        
        public void RemoveStatusEffect(ushort statusEffectId)
        { 
            StatusEffectNetwork statusEffect = GetStatusEffectById(statusEffectId);

            if (!statusEffect.IsValid())
                return;
            
            RemoveStatusEffect(statusEffect);
        }
        private StatusEffectNetwork GetStatusEffectById(ushort id)
        {
            foreach (var statusEffect in _statusEffects)
            {
                if (statusEffect.Value.SessionId() == id)
                    return statusEffect.Value;
            }

            return new StatusEffectNetwork();
        }

        private void RemoveStatusEffect(StatusEffectNetwork statusEffect)
        {
            ushort sessionId = statusEffect.SessionId();
            
            statusEffect.ForceExpire();
            _statusEffects.Remove(sessionId);
            _visualizer.RemoveEffect(sessionId);
            _dirtyFlag = true;
        }

        public void RefreshCache()
        {
            _massMultiplierCashed = 1f;
            _movementSpeedModifierCached = 0;
            _movementSpeedMultiplierCached = 1f;
            _damageOutputModifierCached = 1f;
            _damageTakenModifierCached = 0f;
            _healthPerSecondCached = 0f;
            _leechingPerSecondCached = 0;
            _attackRateModifierCached = 1f;
            _spikeDamageModifierCached = 0f;
            _isReflectiveCached = false;
            _blocksBuffsCached = false;
            _blocksDebuffsCached = false;
            _buffsLastForeverCached = false;
            _blocksCastingBuffsCached = false;
            _blocksCastingDebuffsCached = false;
            _bloodVengeanceChainedEffect = null;
            _disableFiringCached = false;
            _projectileExplodesCached = false;
            _projectileReflectsCached = false;
            _projectileLifeExtendedCached = 0;
            _additionalProjectilesSprayCached = 0;
            _piercesCached = false;

            foreach (KeyValuePair<ushort, StatusEffectNetwork> statusEffectKVP in _statusEffects)
            {
                StatusEffectNetwork statusEffect = statusEffectKVP.Value;
                
                if(statusEffect.HasExpired())
                    continue;

                _massMultiplierCashed *= statusEffect.MassMultiplier();
                _movementSpeedModifierCached += statusEffect.MovementSpeedModifier();
                _movementSpeedMultiplierCached *= statusEffect.MovementSpeedMultiplier();
                _damageOutputModifierCached *= statusEffect.DamageOutputMultiplier();
                _damageTakenModifierCached += statusEffect.DamageTakenModifier();
                _healthPerSecondCached += statusEffect.HealthPerSecond();
                _attackRateModifierCached *= statusEffect.AttackRateMultiplier();
                _spikeDamageModifierCached += statusEffect.SpikeDamageModifier();
                _leechingPerSecondCached += statusEffect.Leeching();
                _additionalProjectilesSprayCached += statusEffect.AdditionalProjectilesSpray();

                if (statusEffect.HealthPerSecond() < 0)
                    _lastDotAppliedBy = statusEffect.OriginPlayer();

                if (statusEffect.IsReflective())
                    _isReflectiveCached = true;

                if (statusEffect.BlocksBuffs())
                    _blocksBuffsCached = true;

                if (statusEffect.BlocksDebuffs())
                    _blocksDebuffsCached = true;

                if (statusEffect.BlocksCastingBuffs())
                    _blocksCastingBuffsCached = true;

                if (statusEffect.BlocksCastingDebuffs())
                    _blocksCastingDebuffsCached = true;

                if (statusEffect.BuffsLastForever())
                    _buffsLastForeverCached = true;

                if (statusEffect.Leeching() > .1f &&
                    (_leechingAppliedByCached == null || !_leechingAppliedByCached.IsAlive)) // Only switch to a new player if the current cache is invalid
                {
                    _leechingAppliedByCached = statusEffect.OriginPlayer();
                }

                if (statusEffect.BloodPact())
                {
                    // Trigger this effect when someone with Blood Pact is killed
                    _bloodVengeanceChainedEffect = statusEffect.GetChainedEffect();
                }

                if (statusEffect.DisableFiring())
                {
                    _disableFiringCached = true;
                }

                if (statusEffect.ProjectileExplodes())
                {
                    _projectileExplodesCached = true;
                }

                if (statusEffect.ProjectileReflects())
                {
                    _projectileReflectsCached = true;
                }

                if (statusEffect.Pierces())
                {
                    _piercesCached = true;
                }

                _projectileLifeExtendedCached += statusEffect.ProjectileLifeExtension();
            }

            // Get indecies
            SortedSet<ushort> indexedIds = new SortedSet<ushort>();
            foreach (KeyValuePair<ushort,StatusEffectNetwork> keyValuePair in _statusEffects)
            {
                indexedIds.Add(keyValuePair.Key);
            }

            _visualizer.Refresh(indexedIds);

            _dirtyFlag = false;
        }

        private void ClearBuffs()
        {
            // Copy to safely enum
            NetworkDictionary<ushort, StatusEffectNetwork> statusEffectsCopy = _statusEffects;
            
            foreach (KeyValuePair<ushort, StatusEffectNetwork> statusEffectKVP in statusEffectsCopy)
            {
                StatusEffectNetwork statusEffect = statusEffectKVP.Value;
                
                if(!statusEffect.IsImmuneToRemoval() && statusEffect.IsBuff())
                    RemoveStatusEffect(statusEffect);
            }
        }

        private void ClearDebuffs()
        {
            // Copy to safely enum
            NetworkDictionary<ushort, StatusEffectNetwork> statusEffectsCopy = _statusEffects;
            
            foreach (KeyValuePair<ushort, StatusEffectNetwork> statusEffectKVP in statusEffectsCopy)
            {
                StatusEffectNetwork statusEffect = statusEffectKVP.Value;
                
                if(!statusEffect.IsImmuneToRemoval() && statusEffect.IsDebuff())
                    RemoveStatusEffect(statusEffect);
            }
        }

        private StatusEffectNetwork StatusEffectAlreadyExists(ushort id)
        {
            foreach (KeyValuePair<ushort, StatusEffectNetwork> statusEffectKVP in _statusEffects)
            {
                StatusEffectNetwork statusEffect = statusEffectKVP.Value;
                
                if (statusEffect.SessionId() == id)
                    return statusEffect;
            }

            return new StatusEffectNetwork();
        }

        /// <summary>
        /// Iterate over all status effects, get death fx
        /// </summary>
        /// <returns></returns>
        public ushort GetDeathFx()
        {
            foreach (KeyValuePair<ushort, StatusEffectNetwork> statusEffectKVP in _statusEffects)
            {
                StatusEffectNetwork statusEffect = statusEffectKVP.Value;
                
                if (statusEffect.DeathFxData())
                {
                    return statusEffect.DeathFxData().SessionId;
                }
            }

            return 0;
        }

        private void Leech()
        {
            if (_leechingPerSecondCached <= 0 || (_leechingAppliedByCached != null && !_leechingAppliedByCached.IsAlive))
                return;
            
            _playerController.CombatController.TakeDamage(_leechingPerSecondCached, _leechingAppliedByCached);
            
            if(_leechingAppliedByCached != null)
                _leechingAppliedByCached.Heal(_leechingPerSecondCached);
        }

        // Server-only, trigger blood pact
        public void BloodPact(PlayerController killer)
        {
            if (_bloodVengeanceChainedEffect == null || !killer.IsAlive)
                return;
            
            killer.StatusEffectController.AddStatusEffect(_bloodVengeanceChainedEffect.Id, _playerController);

            Transform killerTransform = killer.transform;
            PoolManager.Spawn(_bloodVengeanceChainedEffect.DeathFxData.VisualEffectPrefab, killerTransform.position, killerTransform.rotation);
        }
    }
}