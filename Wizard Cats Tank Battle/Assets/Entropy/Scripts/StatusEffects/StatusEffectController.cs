using System.Collections.Generic;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.StatusEffects
{
    public class StatusEffectController : MonoBehaviour
    {
        public StatusEffectDirectory StatusEffectDirectory;
        
        private List<StatusEffect> _statusEffects = new();
        private bool _dirtyFlag;
        private float _movementSpeedModifierCached = 1;
        private float _damageOutputModifierCached = 1;
        private float _defenseModifierCached = 1;
        private float _healthPerSecondCached = 0;

        private const float _refreshRateS = .5f;
        private float _lastRefresh = 0;
        private Player _lastDotAppliedBy;
        
        public List<StatusEffect> StatusEffects => _statusEffects;

        public float MovementSpeedModifier => _movementSpeedModifierCached;
        public float DamageOutputModifier => _damageOutputModifierCached;
        public float DefenseModifier => _defenseModifierCached; // TODO: Implement Defense
        public float HealthPerSecond => _healthPerSecondCached;
        public Player LastDotAppliedBy => _lastDotAppliedBy;

        public void AddStatusEffect(string statusEffectId, Player owner)
        {
            StatusEffect statusEffect = new StatusEffect(this, statusEffectId, owner);
            
            Debug.Log("Adding status effect: " + statusEffect.GetTitle());
            
            StatusEffect existingEffect = StatusEffectAlreadyExists(statusEffect.GetId());

            if (existingEffect == null)
            {
                _statusEffects.Add(statusEffect);
                _dirtyFlag = true;
            }
            else
            {
                existingEffect.SetExpiration();
            }
        }

        public void ClearStatusEffects()
        {
            _statusEffects.Clear();
        }

        private void Update()
        {
            if(Time.time + _refreshRateS >= _lastRefresh)
                CheckLifeOfStatusEffects();
            
            if(_dirtyFlag)
                RefreshCache();

            _lastRefresh = Time.time;
        }
        
        private void CheckLifeOfStatusEffects()
        {
            // Duplicate to safely iterate
            List<StatusEffect> statusEffects = new List<StatusEffect>(_statusEffects);

            foreach (StatusEffect statusEffect in statusEffects)
            {
                if (statusEffect.HasExpired())
                {
                    _statusEffects.Remove(statusEffect);
                    Debug.Log("Removing status effect: " + statusEffect.GetTitle());
                    _dirtyFlag = true;
                }
            }
        }

        private void RefreshCache()
        {
            _movementSpeedModifierCached = 1;
            _damageOutputModifierCached = 1;
            _defenseModifierCached = 1;
            _healthPerSecondCached = 0f;

            foreach (var statusEffect in _statusEffects)
            {
                _movementSpeedModifierCached *= statusEffect.GetMovementSpeedModifier();
                _damageOutputModifierCached *= statusEffect.GetDamageOutputModifier();
                _defenseModifierCached *= statusEffect.GetDefenseModifier();
                _healthPerSecondCached *= statusEffect.GetHealthPerSecond();

                if (statusEffect.GetHealthPerSecond() < 0)
                {
                    _lastDotAppliedBy = statusEffect.GetOriginPlayer();
                }
            }

            _dirtyFlag = false;
        }

        private StatusEffect StatusEffectAlreadyExists(string id)
        {
            foreach (var statusEffect in _statusEffects)
            {
                if (statusEffect.GetId() == id)
                    return statusEffect;
            }

            return null;
        }
    }
}