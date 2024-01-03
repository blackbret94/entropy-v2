using System;
using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.Spells;

namespace Entropy.Scripts.Player
{
    [CreateAssetMenu(fileName = "Class Definition", menuName = "Entropy/Class Definition", order = 1)]
    public class ClassDefinition: ScriptableObject
    {
        public int classId = 0;
        
        [Header("Display")]
        public string className = "DEFAULT";
        public string description;
        public string role;
        
        public Sprite classIcon;
        public Sprite classPortrait;
        
        public Color colorPrimary;
        public Color colorSecondary;
        public Color colorTertiary;
        [Range(1, 5)] 
        public int healthDisplay;
        [Range(1, 5)] 
        public int damageDisplay;
        [Range(1, 5)] 
        public int speedDisplay;
        
        [Range(0f,50f)]
        public int maxHealth = 10;
        [Range(0.2f, 1.5f)]
        public float fireRate = 0.75f;
        [Range(2f, 12f)]
        public float moveSpeed = 8f;
        [Range(0f, 17f)]
        public int damageAmtOnCollision = 5;
        [Range(0f, 7f)]
        public int armor = 2;
        public GameObject Missile;

        public ClassList classList;

        [Header("Ultimates")]
        public Sprite ultimateIcon;
        public SpellData ultimateSpell;
        [Tooltip("Cost to cast ultimate")]
        public int ultimateCost;

        // The class does extra damage to these classes
        public List<ClassDefinition> classCounters;

        private Dictionary<int,ClassDefinition> counterClassIdList;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (counterClassIdList != null)
                return;
            
            counterClassIdList = new Dictionary<int, ClassDefinition>();

            foreach (var classCounter in classCounters)
            {
                counterClassIdList.Add(classCounter.classId, classCounter);
            }
        }

        public bool IsCounter(int classId)
        {
            Init();
            
            return counterClassIdList.ContainsKey(classId);
        }

        public List<ClassDefinition> GetClassesCounteredBy()
        {
            List<ClassDefinition> counteredBy = new();

            foreach (var classDefinition in classList.Classes)
            {
                if(classDefinition.IsCounter(classId))
                    counteredBy.Add(classDefinition);
            }

            return counteredBy;
        }
    }
}