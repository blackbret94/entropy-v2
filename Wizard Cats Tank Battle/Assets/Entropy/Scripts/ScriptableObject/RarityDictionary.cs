using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Rarity Dictionary", menuName = "Entropy/Rarity Dictionary", order = 1)]
    public class RarityDictionary : UnityEngine.ScriptableObject
    {
        public RarityDefinition[] Directory;
        private Dictionary<string, RarityDefinition> _dictionary;
        private Dictionary<Rarity, RarityDefinition> _dictionaryRarity;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            // Create stringId indexed dictionary
            _dictionary = new Dictionary<string, RarityDefinition>();
            _dictionaryRarity = new Dictionary<Rarity, RarityDefinition>();
            
            foreach (RarityDefinition go in Directory)
            {
                _dictionary.Add(go.Id, go);
                _dictionaryRarity.Add(go.Rarity, go);
            }
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// </summary>
        /// <param name="key"></param>
        public RarityDefinition this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.TryGetValue(key, out var value) ? value : Directory[0];
            }
        }

        public RarityDefinition this[Rarity rarity]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionaryRarity.TryGetValue(rarity, out var value) ? value : Directory.First();
            }
        }
    }
}