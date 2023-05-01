using UnityEngine;
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

        [Header("Effects")] 
        public float MovementSpeedModifier = 1f;
        public float DamageOutputModifier = 1f;
        public float DefenseModifier = 1f;
        public float HealthPerSecond = 0f;
    }
}