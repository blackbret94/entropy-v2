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

        public Player GetOriginPlayer()
        {
            return _originPlayer;
        }

        public void SetExpiration()
        {
            _expiration = Time.time + StatusEffectData().TTL;
        }
        
        public float GetExpirationTime()
        {
            return _expiration;
        }

        public bool HasExpired()
        {
            return Time.time > _expiration;
        }
        public string GetId()
        {
            return StatusEffectData().Id;
        }
        
        public string GetTitle()
        {
            return StatusEffectData().Title;
        }

        public string GetDescription()
        {
            return StatusEffectData().Description;
        }

        public float GetMovementSpeedModifier()
        {
            return StatusEffectData().MovementSpeedModifier;
        }

        public float GetDamageOutputModifier()
        {
            return StatusEffectData().DamageOutputModifier;
        }

        public float GetDefenseModifier()
        {
            return StatusEffectData().DefenseModifier;
        }

        public float GetHealthPerSecond()
        {
            return StatusEffectData().HealthPerSecond;
        }

        public Sprite GetIcon()
        {
            return StatusEffectData().EffectIcon;
        }

        private StatusEffectData StatusEffectData()
        {
            return _statusEffectController.StatusEffectDirectory[_id];
        }
    }
}