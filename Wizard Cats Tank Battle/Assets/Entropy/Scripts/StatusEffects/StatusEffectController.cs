using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.UI;

namespace Vashta.Entropy.StatusEffects
{
    public class StatusEffectController : MonoBehaviour
    {
        public StatusEffectDirectory StatusEffectDirectory;
        public StatusEffectPanel StatusEffectPanel;
        
        private Player _player;
        private PlayerStatusEffectVisualizer _visualizer;
        
        private List<StatusEffect> _statusEffects = new();
        private SortedSet<string> _indexedIds = new();
        private bool _dirtyFlag;
        
        private const float _refreshRateS = .5f;
        private float _lastRefresh = 0;
        private Player _lastDotAppliedBy;
        
        // effects
        private float _movementSpeedModifierCached = 0f;
        private float _movementSpeedMultiplierCached = 1f;
        private float _damageOutputModifierCached = 1f;
        private float _damageTakenModifierCached = 0f;
        private float _healthPerSecondCached = 0f;
        private int _leechingPerSecondCached = 0;
        private float _attackRateModifierCached = 1f;
        private float _spikeDamageModifierCached = 0f;
        private bool _isReflectiveCached = false;
        
        // behavior changes
        private bool _blocksBuffsCached = false;
        private bool _blocksDebuffsCached = false;
        private bool _blocksCastingBuffsCached = false;
        private bool _blocksCastingDebuffsCached = false;
        private Player _leechingAppliedByCached;
        private int _bloodPactDamageCached = 0;
        
        public List<StatusEffect> StatusEffects => _statusEffects;

        public float MovementSpeedModifier => _movementSpeedModifierCached;
        public float MovementSpeedMultiplier => _movementSpeedMultiplierCached;
        public float DamageOutputModifier => _damageOutputModifierCached;
        public float DamageTakenModifier => _damageTakenModifierCached;
        public float HealthPerSecond => _healthPerSecondCached;
        public int LeechingPerSecond => _leechingPerSecondCached;
        public float AttackRateModifier => _attackRateModifierCached;
        public float SpikeDamageModifier => _spikeDamageModifierCached;
        public bool IsReflective => _isReflectiveCached;
        public bool BlocksBuffs => _blocksBuffsCached;
        public bool BlocksDebuffs => _blocksDebuffsCached;
        public bool BlocksCastingBuffs => _blocksCastingBuffsCached;
        public bool BlocksCastingDebuffs => _blocksCastingDebuffsCached;
        public Player LeechingAppliedBy => _leechingAppliedByCached;
        public int BloodPactDamage => _bloodPactDamageCached;
        
        
        public Player LastDotAppliedBy => _lastDotAppliedBy;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _visualizer = GetComponent<PlayerStatusEffectVisualizer>();
        }
        
        /// <summary>
        /// Server only, call RPC on all clients
        /// </summary>
        /// <param name="statusEffectId"></param>
        /// <param name="owner"></param>
        public void AddStatusEffect(string statusEffectId, Player owner)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _player.photonView.RPC("RPCAddStatusEffect", RpcTarget.All, statusEffectId, owner?owner.GetId():null);
            }
        }
        
        /// <summary>
        /// Called on all clients to add status effect
        /// </summary>
        /// <param name="statusEffectId"></param>
        /// <param name="playerId"></param>
        [PunRPC]
        public void RPCAddStatusEffect(string statusEffectId, int playerId)
        {
            Player owner = Player.GetPlayerById(playerId);
            StatusEffect statusEffect = new StatusEffect(this, statusEffectId, owner);
            
            // If a buff and buffs are blocked, return
            if (statusEffect.IsBuff() && _blocksBuffsCached)
                return;
            
            // If a debuff and debuffs are blocked, return
            if (statusEffect.IsDebuff() && _blocksDebuffsCached)
                return;
            
            // Clear buffs if this blocks buffs
            if(statusEffect.BlocksBuffs())
                ClearBuffs();
            
            // Clear debuffs if this blocks debuffs
            if(statusEffect.BlocksDebuffs())
                ClearDebuffs();

            // Alert if local player
            if (_player.IsLocal)
            {
                // Show panel
                GameManager.GetInstance().ui.PowerUpPanel.SetText(statusEffect.Title(), statusEffect.Description(),
                    statusEffect.Color(), statusEffect.Icon());

                // Play fx
                if(statusEffect.Sfx())
                    AudioManager.Play2D(statusEffect.Sfx());
            }

            // Check if status effect already exists
            StatusEffect existingEffect = StatusEffectAlreadyExists(statusEffect.Id());

            if (statusEffect.ApplyInstantly())
            {
                // Only run on master player
                if (PhotonNetwork.IsMasterClient)
                {
                    int healthPerSecond = Mathf.RoundToInt(statusEffect.HealthPerSecond());

                    if (healthPerSecond > 0)
                    {
                        _player.Heal(healthPerSecond);
                    }
                    else if (healthPerSecond < 0)
                    {
                        _player.TakeDamage(healthPerSecond, statusEffect.OriginPlayer());
                    }
                }

                // Do NOT add as status effect if it is supposed to be instantly applied
                return;
            }

            if (existingEffect == null)
            {
                // If it doesn't exist, add it
                _statusEffects.Add(statusEffect);
                _indexedIds.Add(statusEffect.Id());
                _dirtyFlag = true;
            }
            else
            {
                // If it exists, update TTL
                existingEffect.SetExpiration();
                existingEffect.SetFresh(true);
            }
            
            StatusEffectPanel.ForceRefresh();
        }

        public void ClearStatusEffects()
        {
            _statusEffects.Clear();
            _indexedIds.Clear();
            _dirtyFlag = true;
            StatusEffectPanel.ResetSlots();
        }

        private void Update()
        {
            if(Time.time + _refreshRateS >= _lastRefresh)
                CheckLifeOfStatusEffects();
            
            if(_dirtyFlag)
                RefreshCache();
        }
        
        private void CheckLifeOfStatusEffects()
        {
            // Duplicate to safely iterate
            List<StatusEffect> statusEffects = new List<StatusEffect>(_statusEffects);

            foreach (StatusEffect statusEffect in statusEffects)
            {
                if (statusEffect.HasExpired())
                    RemoveStatusEffect(statusEffect);
            }
            
            _lastRefresh = Time.time;
        }

        private void RemoveStatusEffect(StatusEffect statusEffect)
        {
            _statusEffects.Remove(statusEffect);
            _indexedIds.Remove(statusEffect.Id());
            _dirtyFlag = true;
        }

        public void RefreshCache()
        {
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
            _blocksCastingBuffsCached = false;
            _blocksCastingDebuffsCached = false;
            _bloodPactDamageCached = 0;

            foreach (var statusEffect in _statusEffects)
            {
                if(statusEffect.HasExpired())
                    continue;
                
                _movementSpeedModifierCached += statusEffect.MovementSpeedModifier();
                _movementSpeedMultiplierCached *= statusEffect.MovementSpeedMultiplier();
                _damageOutputModifierCached *= statusEffect.DamageOutputMultiplier();
                _damageTakenModifierCached += statusEffect.DamageTakenModifier();
                _healthPerSecondCached += statusEffect.HealthPerSecond();
                _attackRateModifierCached *= statusEffect.AttackRateMultiplier();
                _spikeDamageModifierCached += statusEffect.SpikeDamageModifier();
                _bloodPactDamageCached += statusEffect.BloodPactDamage();
                _leechingPerSecondCached += statusEffect.Leeching();

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

                if (statusEffect.Leeching() > .1f &&
                    (_leechingAppliedByCached == null || !_leechingAppliedByCached.gameObject.activeSelf))
                {
                    _leechingAppliedByCached = statusEffect.OriginPlayer();
                }
            }

            _visualizer.Refresh(_indexedIds);
            _dirtyFlag = false;
        }

        private void ClearBuffs()
        {
            // Copy to safely enum
            List<StatusEffect> statusEffectsCopy = new List<StatusEffect>(_statusEffects);
            
            foreach (var statusEffect in statusEffectsCopy)
            {
                if(statusEffect.IsBuff())
                    RemoveStatusEffect(statusEffect);
            }
        }

        private void ClearDebuffs()
        {
            // Copy to safely enum
            List<StatusEffect> statusEffectsCopy = new List<StatusEffect>(_statusEffects);
            
            foreach (var statusEffect in statusEffectsCopy)
            {
                if(statusEffect.IsDebuff())
                    RemoveStatusEffect(statusEffect);
            }
        }

        private StatusEffect StatusEffectAlreadyExists(string id)
        {
            foreach (var statusEffect in _statusEffects)
            {
                if (statusEffect.Id() == id)
                    return statusEffect;
            }

            return null;
        }

        /// <summary>
        /// Iterate over all status effects, get death fx
        /// </summary>
        /// <returns></returns>
        public GameObject GetDeathFx()
        {
            foreach (var statusEffect in _statusEffects)
            {
                StatusEffectData data = StatusEffectDirectory[statusEffect.Id()];

                if (data && data.DeathFx != null)
                {
                    return data.DeathFx;
                }
                else
                {
                    Debug.Log("Death fx not found! " + data.Title);
                }
            }

            return null;
        }

        public void Leech()
        {
            if (_leechingPerSecondCached <= 0)
                return;
            
            _player.TakeDamage(_leechingPerSecondCached, _leechingAppliedByCached);
            
            if (_leechingAppliedByCached != null && _leechingAppliedByCached.IsAlive)
            {
                _leechingAppliedByCached.Heal(_leechingPerSecondCached);
            }
            
        }

        public void BloodPact(Player killer)
        {
            if (_bloodPactDamageCached <= 0)
                return;
            
            if (killer.IsAlive)
            {
                killer.TakeDamage(_bloodPactDamageCached, _player);
            }
        }
    }
}