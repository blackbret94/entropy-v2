using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.Spells
{
    [CreateAssetMenu(fileName = "Spell", menuName = "Entropy/Spell", order=1)]
    public class SpellData : ScriptableObjectWithID
    {
        [Header("Definition")]
        public Sprite SpellIcon;
        public string Title;
        [Tooltip("This text appears when the status effect is applied")]
        public string AppliedDescription;
        [Tooltip("This text appears in the class selection menu")]
        public string ClassDescription;
        public Color Color;
        
        [Header("Behavior")]
        [Tooltip("1 = 1 second")]
        public float TTL;
        [Tooltip("Ignores TTL and is triggered instantly")]
        public bool IsInstant;
        public float Radius;
        [Tooltip("Does it follow the player or remain still?")]
        public bool IsStationary = true;
        
        [Header("Cast")]
        [Tooltip("Delay apply effect for a set period of time")]
        public float DelayEffect;
        
        public int HealAlliesOnCast = 0;
        public GameObject HealAlliesEffect;
        [FormerlySerializedAs("CastStatusEffectToApplyToAllies")] public StatusEffectData CastStatusEffectAllies;
        
        public int DamageEnemiesOnCast = 0;
        public GameObject DamageEnemiesEffect;
        [FormerlySerializedAs("CastStatusEffectToApplyToEnemies")] public StatusEffectData CastStatusEffectEnemies;
        
        [FormerlySerializedAs("ActiveStatusEffectToApplyToAllies")]
        [Header("While Active")]
        [FormerlySerializedAs("StatusEffectToApplyToAllies")] 
        public StatusEffectData ActiveStatusEffectAllies;
        [FormerlySerializedAs("ActiveStatusEffectToApplyToEnemies")] [FormerlySerializedAs("StatusEffectToApplyToEnemies")] 
        public StatusEffectData ActiveStatusEffectEnemies;
        public bool DestroyEnemyProjectilesWhileActive;
        public float IncreaseAlliedProjectileSpeedWhileActive = 0;
        
        [Header("Graphics And Audio")] 
        public GameObject EffectToSpawn;
        [Tooltip("Sound effect that plays when it is cast")]
        public AudioClip Sfx;
    }
}