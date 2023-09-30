using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.StatusEffects
{
    public class StatusEffect
    {
        private StatusEffectController _statusEffectController;
        private string ID { get; }
        private Player _originPlayer { get; }
        private float _expiration;
        private bool _isFresh = false;

        private StatusEffectData _data = null;

        public StatusEffect(StatusEffectController statusEffectController, string id, Player originPlayer)
        {
            _statusEffectController = statusEffectController;
            ID = id;
            _originPlayer = originPlayer;
            SetExpiration();
            _isFresh = true;
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
            return ID;
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

        public bool BlocksCastingBuffs()
        {
            return StatusEffectData().BlocksFromCastingBuffs;
        }

        public bool BlocksCastingDebuffs()
        {
            return StatusEffectData().BlocksFromCastingDeuffs;
        }

        public int Leeching()
        {
            return StatusEffectData().LeechingPerSecond;
        }

        public int BloodPactDamage()
        {
            return StatusEffectData().BloodPactDamage;
        }

        public AudioClip Sfx()
        {
            return StatusEffectData().Sfx;
        }

        public bool ApplyInstantly()
        {
            return StatusEffectData().ApplyInstantly;
        }
        
        private StatusEffectData StatusEffectData()
        {
            // If cached, return cache
            if (_data != null)
                return _data;
            
            // Attempt to locate
            _data = _statusEffectController.StatusEffectDirectory[ID];

            if (_data == null)
            {
                Debug.LogError("Could not find status effect with ID: " + ID);
            }

            return _data;
        }

        public bool IsFresh()
        {
            return _isFresh;
        }

        public void SetFresh( bool fresh)
        {
            _isFresh = fresh;
        }
    }
}