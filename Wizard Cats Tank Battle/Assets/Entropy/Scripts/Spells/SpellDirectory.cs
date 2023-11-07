using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.Spells
{
    [CreateAssetMenu(fileName = "SpellDirectory", menuName = "Entropy/SpellDirectory", order=1)]
    public class SpellDirectory : UnityEngine.ScriptableObject
    {
        public SpellData[] Directory;
        private Dictionary<string, SpellData> _dictionary;
        
        void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }

        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            _dictionary = new Dictionary<string, SpellData>();
         
            foreach (SpellData spell in Directory)
                _dictionary.Add(spell.Id,spell);
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// audioDirectory["shoot"]
        /// </summary>
        /// <param name="key"></param>
        public SpellData this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
    }
}