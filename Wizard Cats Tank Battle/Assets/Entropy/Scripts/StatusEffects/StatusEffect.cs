using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.StatusEffects
{
    public class StatusEffect
    {
        private StatusEffectController _statusEffectController;
        private string ID { get; }
        private Player _originPlayer { get; }
        private float _expiration;
        private bool _isFresh = false;
        private bool _forceExpire = false;
        private float _timeCreated;

        private StatusEffectData _data = null;

        public StatusEffect(StatusEffectController statusEffectController, string id, Player originPlayer)
        {
            _statusEffectController = statusEffectController;
            ID = id;
            _originPlayer = originPlayer;
            SetExpiration();
            _isFresh = true;
            _timeCreated = Time.time;
        }

        public float GetAge()
        {
            return Time.time - _timeCreated;
        }
        
        public void ForceExpire()
        {
            _forceExpire = true;
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
            if (_forceExpire)
                return true;
            
            if (_data.TTL < 0) 
                return false;
            
            return Time.time > _expiration;
        }

        public float GetTimeLeft()
        {
            if (_forceExpire)
                return 0;
            
            if (_data.TTL < 0)
                return 100f;
            
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

        public float MassMultiplier()
        {
            return StatusEffectData().MassMultiplaier;
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

        public VisualEffect ApplyFxData()
        {
            return _data.ApplyFxData;
        }
        
        public VisualEffect DeathFxData()
        {
            return _data.DeathFxData;
        }

        public bool IsImmuneToRemoval()
        {
            return StatusEffectData().ImmuneToRemoval;
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

        public bool BloodPact()
        {
            return StatusEffectData().BloodPact;
        }

        public AudioClip Sfx()
        {
            return StatusEffectData().Sfx;
        }

        public bool ApplyInstantly()
        {
            return StatusEffectData().ApplyInstantly;
        }

        public bool BuffsLastForever()
        {
            return StatusEffectData().BuffsLastForever;
        }

        public StatusEffectData GetChainedEffect()
        {
            return StatusEffectData().ChainedStatusEffect;
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

        public bool DisableFiring()
        {
            return _data.DisableFiring;
        }

        public bool ProjectileExplodes()
        {
            return _data.ProjectilesExplode;
        }

        public bool ProjectileReflects()
        {
            return _data.ProjectileReflects;
        }

        public float ProjectileLifeExtension()
        {
            return _data.ProjectileLifeExtended;
        }

        public int AdditionalProjectilesSpray()
        {
            return _data.AdditionalProjectilesSpray;
        }

        public bool Pierces()
        {
            return _data.Pierces;
        }
    }
}