using System.Collections.Generic;
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

        public void Cast(Player caster)
        {
            List<Player> alliesList;
            List<Player> enemiesList;
            
            GetPlayers(caster, out alliesList, out enemiesList);

            //Apply effects to allies
            foreach (Player player in alliesList)
            {
                player.Heal(HealAlliesOnCast);
                
                if(CastStatusEffectAllies)
                    player.ApplyStatusEffect(CastStatusEffectAllies.Id, caster.GetId());
            }
            
            // Apply effects to enemies
            foreach (Player player in enemiesList)
            {
                player.TakeDamage(DamageEnemiesOnCast, caster);
                
                if(CastStatusEffectEnemies)
                    player.ApplyStatusEffect(CastStatusEffectEnemies.Id, caster.GetId());
            }
            
            // Place field
            if (SpawnsField)
            {
                GameObject field = PoolManager.Spawn(SpellFieldPrefab, caster.transform.position, Quaternion.identity);
                SpellField spellField = field.GetComponent<SpellField>();
                spellField.Init(caster, this);
            }
        }

        private void GetPlayers(Player caster, out List<Player> alliesList, out List<Player> enemiesList)
        {
            alliesList = new List<Player>();
            enemiesList = new List<Player>();
            
            List<Player> allPlayers = Player.GetAllPlayers;
            int casterTeamIndex = caster.GetView().GetTeam();

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
    }
}