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
        public string Description;
        [Tooltip("1 = 1 second")]
        public float TTL;
        public Color Color;
        [Tooltip("Optional, for unique mechanics")]
        public int PowerupId;

        [Header("Effects")] [Tooltip("Multiplies")]
        public float MovementSpeedMultiplier = 1f;
        [Tooltip("Sums")]
        public float MovementSpeedModifier = 0f;
        [FormerlySerializedAs("DamageOutputModifier")] [Tooltip("Multiplies")]
        public float DamageOutputMultiplier = 1f;
        [FormerlySerializedAs("AttackRateModifier")] [Tooltip("Multiplies")]
        public float AttackRateMultiplier = 1f;
        [Tooltip("Sums")]
        public float SpikeDamageModifier = 0f;
        [Tooltip("Sums")]
        public float DefenseModifier = 0f;
        [Tooltip("Sums")]
        public float HealthPerSecond = 0f;
        [Tooltip("Toggles reflective bubble")]
        public bool IsReflective = false;
    }
}