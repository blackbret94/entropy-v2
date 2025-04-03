using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Projectile Data", menuName = "Entropy/Projectile", order = 1)]
    public class ProjectileData: UnityEngine.ScriptableObject
    {
        public int ProjectileId;
        
        [Header("Stats")]
        public int BaseDamage;
        public float BaseLifetime;
        public float BaseSpeed;
        public int BaseBounces;
         
        [Header("Visuals")]
        public GameObject ProjectilePrefab;
        public GameObject CastFx;
        public GameObject HitFx;
        public GameObject ExplosionFx;
        public GameObject ExplosionFxLarge;
        
        [Header("Sounds")]
        public ScriptableAudioClipList HitSfx;
        public ScriptableAudioClipList CastSfx;
        public ScriptableAudioClipList BounceSfx;
        
        [Header("Death")]
        public VisualEffectData deathFxData;
        
        [Header("Class Mechanics")]
        public StatusEffectData StatusEffectOnEnemy;
        [Range(0,1)]
        public float StatusEffectOnEnemyChance;
        public StatusEffectData StatusEffectOnAlly;
        [Range(0,1)]
        public float StatusEffectOnAllyChance;
    }
}