using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.StatusEffects
{
    public struct StatusEffectNetwork : INetworkStruct
    {
        // TODO: string - maybe I just need sessionId instead?
        private int _sessionId;
        private PlayerRef _player;
        private float _expiration;
        private NetworkBool _forceExpire;
        private float _timeCreated;
        private NetworkBool _isValid; // Not sure if I need this
        
        public void Reset()
        {
            _sessionId = 0;
            _player = new PlayerRef();
            _expiration = 0;
            _forceExpire = false;
            _timeCreated = 0;
            _isValid = false;
        }

        public void Set(int sessionId, PlayerRef player, float expiration, bool forceExpire, float timeCreated,
            bool isValid)
        {
            _sessionId = sessionId;
            _player = player;
            _expiration = expiration;
            _forceExpire = forceExpire;
            _timeCreated = timeCreated;
            _isValid = isValid;
        }
        
        public float GetAge()
        {
            return Time.time - _timeCreated;
        }
        
        public void ForceExpire()
        {
            _forceExpire = true;
        }

        public PlayerController OriginPlayer()
        {
            NetworkManagerCustom networkManagerCustom = NetworkManagerCustom.GetInstance();
            return networkManagerCustom.GetPlayerGameObject(_player);
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
            
            if (StatusEffectData().TTL < 0) 
                return false;
            
            return Time.time > _expiration;
        }

        public float GetTimeLeft()
        {
            if (_forceExpire)
                return 0;
            
            if (StatusEffectData().TTL < 0)
                return 100f;
            
            return _expiration - Time.time;
        }
        
        public int SessionId()
        {
            return _sessionId;
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

        public VisualEffectData ApplyFxData()
        {
            return StatusEffectData().ApplyFxData;
        }
        
        public VisualEffectData DeathFxData()
        {
            return StatusEffectData().DeathFxData;
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
            // Attempt to locate
            StatusEffectData data = GameDataSet.Get().StatusEffectDirectory.GetBySessionId(_sessionId);

            if (data == null)
            {
                Debug.LogError("Could not find status effect with Session ID: " + _sessionId);
            }

            return data;
        }

        public bool DisableFiring()
        {
            return StatusEffectData().DisableFiring;
        }

        public bool ProjectileExplodes()
        {
            return StatusEffectData().ProjectilesExplode;
        }

        public bool ProjectileReflects()
        {
            return StatusEffectData().ProjectileReflects;
        }

        public float ProjectileLifeExtension()
        {
            return StatusEffectData().ProjectileLifeExtended;
        }

        public int AdditionalProjectilesSpray()
        {
            return StatusEffectData().AdditionalProjectilesSpray;
        }

        public bool Pierces()
        {
            return StatusEffectData().Pierces;
        }
    }
}