using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Team Dictionary", menuName = "Entropy/Team Dictionary", order = 1)]
    public class TeamDefinitionDictionary : UnityEngine.ScriptableObject
    {
        public TeamDefinition[] Directory;
        private Dictionary<string, TeamDefinition> _dictionary;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            // Create stringId indexed dictionary
            _dictionary = new Dictionary<string, TeamDefinition>();
            
            foreach (TeamDefinition go in Directory)
            {
                _dictionary.Add(go.TeamId, go);
            }
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// </summary>
        /// <param name="key"></param>
        public TeamDefinition this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.TryGetValue(key, out var value) ? value : GetRandom();
            }
        }
        
        // Need to eventually take game modes into account here
        public TeamDefinition GetRandom()
        {
            return _dictionary.ElementAt(Random.Range(0, _dictionary.Count)).Value;
        }
    }
}