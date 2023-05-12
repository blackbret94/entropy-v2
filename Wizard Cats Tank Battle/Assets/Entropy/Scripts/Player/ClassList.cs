using System.Collections.Generic;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    [CreateAssetMenu(fileName = "Class List", menuName = "Entropy/ClassList", order = 1)]
    public class ClassList: ScriptableObject
    {
        public List<ClassDefinition> Classes;
        private Dictionary<int, ClassDefinition> _dictionary;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;

            _dictionary = new Dictionary<int, ClassDefinition>();
         
            foreach (ClassDefinition classDefinition in Classes)
                _dictionary.Add(classDefinition.classId,classDefinition);
        }
        
        public ClassDefinition RandomClass()
        {
            return Classes[Random.Range(0, Classes.Count)];
        }
        
        public ClassDefinition this[int key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
    }
}