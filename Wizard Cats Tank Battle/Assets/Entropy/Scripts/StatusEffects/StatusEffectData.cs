using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.StatusEffects
{
    [CreateAssetMenu(fileName = "Status Effect", menuName = "Entropy/Status Effect", order=1)]
    public class StatusEffectData : ScriptableObjectWithID
    {
        public Sprite EffectIcon;
        public int MaxStack = 1;
        public string Title;
        [FormerlySerializedAs("Description")] [Tooltip("This text appears when the status effect is applied")]
        public string AppliedDescription;
        [Tooltip("This text appears in the class selection menu")]
        public string ClassDescription;
        [Tooltip("1 = 1 second")]
        public float TTL;
        public Color Color;
        [Tooltip("Optional, for unique mechanics")]
        public int PowerupId;
        [Tooltip("Adds a red outline around the status effect to show it is bad")]
        public bool IsDebuff;
        [Tooltip("Sound effect that plays when it is applied")]
        public AudioClip Sfx;
        [Tooltip("Effect to play if the player is killed by this status effect.  Optional.")]
        public GameObject DeathFx;
        [Tooltip("Doesn't add the status effect, but applies 1s of immediate effects.")]
        public bool ApplyInstantly = false;
        [Tooltip("Overrides the Voidstalker's ability to remove buffs and debuffs")]
        public bool ImmuneToRemoval;

        [Header("Effects")] 
        [Tooltip("Multiplies")]
        public float MassMultiplaier = 1f;
        [Tooltip("Multiplies")]
        public float MovementSpeedMultiplier = 1f;
        [Tooltip("Sums")]
        public float MovementSpeedModifier = 0f;
        [FormerlySerializedAs("DamageOutputModifier")] [Tooltip("Multiplies")]
        public float DamageOutputMultiplier = 1f;
        [FormerlySerializedAs("AttackRateModifier")] [Tooltip("Multiplies")]
        public float AttackRateMultiplier = 1f;
        [Tooltip("Sums")]
        public float SpikeDamageModifier = 0f;
        [FormerlySerializedAs("DefenseModifier")] [Tooltip("Sums")]
        public float DamageTakenModifier = 0f;
        [Tooltip("Sums")]
        public float HealthPerSecond = 0f;
        [Tooltip("Damage done heals the caster")]
        public int LeechingPerSecond;
        [Tooltip("Toggles reflective bubble")]
        public bool IsReflective = false;
        
        [Header("Behavior Changes")]
        [Tooltip("Blocks the player from being buffed")]
        public bool BlocksBuffs;
        [Tooltip("Blocks the player from being debuffed")]
        public bool BlocksDebuffs;
        [Tooltip("Blocks the player from buffing their allies")]
        public bool BlocksFromCastingBuffs;
        [Tooltip("Blocks the player from debuffing their enemies")]
        public bool BlocksFromCastingDeuffs;
        [Tooltip("Enables Blood Pact, which triggers a chained status effect upon death")]
        public bool BloodPact;
        [Tooltip("Optional, allows chaining of status effects")]
        public StatusEffectData ChainedStatusEffect = null;

        [Tooltip("Does not remove buffs while active")]
        public bool BuffsLastForever;
    }
}