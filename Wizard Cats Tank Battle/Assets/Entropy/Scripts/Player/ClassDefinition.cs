using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    [CreateAssetMenu(fileName = "Class Definition", menuName = "Entropy/Class Definition", order = 1)]
    public class ClassDefinition: ScriptableObject
    {
        public int classId = 0;
        public string className = "DEFAULT";
        public string description;
        public string role;
        [Range(0f,22f)]
        public int maxHealth = 10;
        [Range(0.2f, 1.2f)]
        public float fireRate = 0.75f;
        [Range(2f, 12f)]
        public float moveSpeed = 8f;
        [Range(0f, 17f)]
        public int damageAmtOnCollision = 5;
        [Range(0f, 7f)]
        public int armor = 2;
        public Sprite classIcon;
        public Sprite classPortrait;
        public Color color;
        public GameObject Missile;
        public Color colorPrimary;
        public Color colorSecondary;
        public Color colorTertiary;

        public ClassList classList;
        

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