using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vashta.Entropy.UI.MapSelection
{
    [CreateAssetMenu(fileName = "MapDefinitionDictionary", menuName = "Entropy/MapDefinitionDictionary", order = 1)]
    public class MapDefinitionDictionary : UnityEngine.ScriptableObject
    {
        public MapDefinition[] Directory;
        private Dictionary<string, MapDefinition> _dictionary;
        private Dictionary<string, MapDefinition> _dictionaryByName;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            _dictionary = new Dictionary<string, MapDefinition>();
            _dictionaryByName = new Dictionary<string, MapDefinition>();


            foreach (MapDefinition go in Directory)
            {
                _dictionary.Add(go.Id, go);
                _dictionaryByName.Add(go.Title, go);
            }
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// audioDirectory["shoot"]
        /// </summary>
        /// <param name="key"></param>
        public MapDefinition this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.TryGetValue(key, out var value) ? value : GetRandom();
            }
        }

        public MapDefinition GetByName(string mapName)
        {
            return _dictionary.TryGetValue(mapName, out var value) ? value : GetRandom();
        }
        
        // Need to eventually take game modes into account here
        public MapDefinition GetRandom()
        {
            return _dictionary.ElementAt(Random.Range(0, _dictionary.Count)).Value;
        }
    }
}