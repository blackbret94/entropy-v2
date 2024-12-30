using System.Collections.Generic;
using Entropy.Scripts.Player;
using TanksMP;
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
        [Tooltip("Enables a field that can follow the caster or remain still")]
        public bool SpawnsField;
        [Tooltip("Does it follow the player or remain still?")]
        public bool IsStationary = true;

        [Header("Cast")] 
        public GameObject SpellFieldPrefab;
        
        [Tooltip("Delay apply effect for a set period of time")]
        public float DelayEffect;
        
        public int HealAlliesOnCast = 0;
        public GameObject HealAlliesEffect;
        [FormerlySerializedAs("CastStatusEffectToApplyToAllies")] public StatusEffectData CastStatusEffectAllies;
        
        public int DamageEnemiesOnCast = 0;
        public GameObject DamageEnemiesEffect;
        [FormerlySerializedAs("CastStatusEffectToApplyToEnemies")] public StatusEffectData CastStatusEffectEnemies;
        public VisualEffect EnemyDeathEffect;
        
        [FormerlySerializedAs("ActiveStatusEffectToApplyToAllies")]
        [Header("While Active")]
        [FormerlySerializedAs("StatusEffectToApplyToAllies")] 
        public StatusEffectData ActiveStatusEffectAllies;
        [FormerlySerializedAs("ActiveStatusEffectToApplyToEnemies")] [FormerlySerializedAs("StatusEffectToApplyToEnemies")] 
        public StatusEffectData ActiveStatusEffectEnemies;
        public bool DestroyEnemyProjectilesWhileActive;
        public float IncreaseAlliedProjectileSpeedWhileActive = 0;

        [Header("Graphics And Audio")] 
        public Vector3 CastEffectOffset;
        public GameObject EffectToSpawn;
        [Tooltip("Sound effect that plays when it is cast")]
        public AudioClip Sfx;
        [Tooltip("Visual effect that spawns when a projectile hits this field")]
        public GameObject FieldCollisionVfxProjectile;
        [Tooltip("Audio to play when a projectile hits this field")]
        public AudioClip FieldHitSfxProjectile;
        [Tooltip("Visual effect that spawns when a character hits this field")]
        public GameObject FieldCollisionVfxCharacter;
        [Tooltip("Audio to play when a character hits this field")]
        public AudioClip FieldHitSfxCharacter;
        public bool VfxPlayForEnemies = true;
        public bool VfxPlayForAllies = true;
        public bool AudioPlayForEnemies = true;
        public bool AudioPlayForAllies = true;

        public void Cast(Player caster)
        {
            List<Player> alliesList;
            List<Player> enemiesList;
            
            GetPlayers(caster, out alliesList, out enemiesList);
            
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);

            //Apply effects to allies
            foreach (Player player in alliesList)
            {
                player.Heal(HealAlliesOnCast);

                if (CastStatusEffectAllies)
                {
                    player.ApplyStatusEffect(CastStatusEffectAllies.Id, caster.GetId());
                    PoolManager.Spawn(HealAlliesEffect, player.transform.position + Vector3.up, rotation);
                }
            }
            
            // Apply effects to enemies
            string deathFxId = EnemyDeathEffect ? EnemyDeathEffect.Id : "";
            foreach (Player player in enemiesList)
            {
                player.CombatController.TakeDamage(DamageEnemiesOnCast, caster, true, deathFxId);

                if (CastStatusEffectEnemies)
                {
                    player.ApplyStatusEffect(CastStatusEffectEnemies.Id, caster.GetId());
                    PoolManager.Spawn(DamageEnemiesEffect, player.transform.position + Vector3.up, rotation);
                }
            }
            
            // Place field
            if (SpawnsField)
            {
                GameObject field = PoolManager.Spawn(SpellFieldPrefab, caster.transform.position + Vector3.up, Quaternion.identity);
                SpellField spellField = field.GetComponent<SpellField>();
                spellField.Init(caster, this);
            }
            else
            {
                // Create effect as one-off
                if (EffectToSpawn)
                {
                    PoolManager.Spawn(EffectToSpawn, caster.transform.position + CastEffectOffset + Vector3.up*.1f, rotation);
                }
            }
        }

        private void GetPlayers(Player caster, out List<Player> alliesList, out List<Player> enemiesList)
        {
            alliesList = new List<Player>();
            enemiesList = new List<Player>();
            
            List<Player> allPlayers = PlayerList.GetAllPlayers;
            int casterTeamIndex = caster.GetView().GetTeam();
            
            // DrawWireSphere(caster.transform.position, Radius, Color.red, 5f);

            foreach (Player player in allPlayers)
            {
                // check if alive
                if (!player.IsAlive)
                    continue;
                
                // check distance
                float dist = Vector3.Distance(caster.transform.position, player.transform.position);
                if (dist > Radius)
                    continue;
                
                // check team id
                if (casterTeamIndex == player.GetView().GetTeam())
                {
                    // Add to ally list
                    alliesList.Add(player);
                }
                else
                {
                    // Add to enemy list
                    enemiesList.Add(player);
                }
            }
        }
        
        // public static void DrawWireSphere(Vector3 center, float radius, Color color, float duration, int quality = 3)
        // {
        //     quality = Mathf.Clamp(quality, 1, 10);
        //
        //     int segments = quality << 2;
        //     int subdivisions = quality << 3;
        //     int halfSegments = segments >> 1;
        //     float strideAngle = 360F / subdivisions;
        //     float segmentStride = 180F / segments;
        //
        //     Vector3 first;
        //     Vector3 next;
        //     for (int i = 0; i < segments; i++)
        //     {
        //         first = (Vector3.forward * radius);
        //         first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.right) * first;
        //
        //         for (int j = 0; j < subdivisions; j++)
        //         {
        //             next = Quaternion.AngleAxis(strideAngle, Vector3.up) * first;
        //             UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
        //             first = next;
        //         }
        //     }
        //
        //     Vector3 axis;
        //     for (int i = 0; i < segments; i++)
        //     {
        //         first = (Vector3.forward * radius);
        //         first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.up) * first;
        //         axis = Quaternion.AngleAxis(90F, Vector3.up) * first;
        //
        //         for (int j = 0; j < subdivisions; j++)
        //         {
        //             next = Quaternion.AngleAxis(strideAngle, axis) * first;
        //             UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
        //             first = next;
        //         }
        //     }
        // }
    }
}