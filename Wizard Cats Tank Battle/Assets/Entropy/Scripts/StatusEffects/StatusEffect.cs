using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.StatusEffects
{
    public class StatusEffect
    {
        private StatusEffectController _statusEffectController;
        private string _id;
        private Player _originPlayer; // How to serialize this?
        private float _expiration;

        public StatusEffect(StatusEffectController statusEffectController, string id, Player originPlayer)
        {
            _statusEffectController = statusEffectController;
            _id = id;
            _originPlayer = originPlayer;
            SetExpiration();
        }

        public Player OriginPlayer()
        {
            return _originPlayer;
        }

        public void SetExpiration()
        {
            _expiration = Time.time + StatusEffectData().TTL;
        }
        
        public float ExpirationTime()
        {
            return _expiration;
        }

        public bool HasExpired()
        {
            return Time.time > _expiration;
        }

        public float GetTimeLeft()
        {
            return _expiration - Time.time;
        }
        
        public string Id()
        {
            return StatusEffectData().Id;
        }
        
        public string Title()
        {
            return StatusEffectData().Title;
        }

        public string Description()
        {
            return StatusEffectData().AppliedDescription;
        }

        public float MovementSpeedModifier()
        {
            return StatusEffectData().MovementSpeedModifier;
        }

        public float MovementSpeedMultiplier()
        {
            return StatusEffectData().MovementSpeedMultiplier;
        }

        public float DamageOutputMultiplier()
        {
            return StatusEffectData().DamageOutputMultiplier;
        }

        public float DamageTakenModifier()
        {
            return StatusEffectData().DamageTakenModifier;
        }

        public float HealthPerSecond()
        {
            return StatusEffectData().HealthPerSecond;
        }

        public Sprite Icon()
        {
            return StatusEffectData().EffectIcon;
        }

        public Color Color()
        {
            return StatusEffectData().Color;
        }

        public int PowerupId()
        {
            return StatusEffectData().PowerupId;
        }

        public float AttackRateMultiplier()
        {
            return StatusEffectData().AttackRateMultiplier;
        }

        public float SpikeDamageModifier()
        {
            return StatusEffectData().SpikeDamageModifier;
        }

        public bool IsReflective()
        {
            return StatusEffectData().IsReflective;
        }

        public bool IsDebuff()
        {
            return StatusEffectData().IsDebuff;
        }

        public bool IsBuff()
        {
            return !StatusEffectData().IsDebuff;
        }

        public bool BlocksBuffs()
        {
            return StatusEffectData().BlocksBuffs;
        }

        public bool BlocksDebuffs()
        {
            return StatusEffectData().BlocksDebuffs;
        }

        public AudioClip Sfx()
        {
            return StatusEffectData().Sfx;
        }
        
        private StatusEffectData StatusEffectData()
        {
            return _statusEffectController.StatusEffectDirectory[_id];
        }
    }
}