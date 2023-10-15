using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Game Mode Dictionary", menuName = "Entropy/Game Mode Dictionary", order = 1)]
    public class GameModeDictionary : UnityEngine.ScriptableObject
    {
        public GameModeDefinition[] Directory;
        private Dictionary<string, GameModeDefinition> _dictionary;
        private Dictionary<TanksMP.GameMode, GameModeDefinition> _dictionaryByGameMode;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null && _dictionaryByGameMode != null) 
                return;
            
            // Create stringId indexed dictionary
            _dictionary = new Dictionary<string, GameModeDefinition>();
            
            // Create GameMode indexed dictionary
            _dictionaryByGameMode = new Dictionary<TanksMP.GameMode, GameModeDefinition>();


            foreach (GameModeDefinition go in Directory)
            {
                _dictionary.Add(go.Id, go);
                _dictionaryByGameMode.Add(go.GameMode, go);
            }
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// </summary>
        /// <param name="key"></param>
        public GameModeDefinition this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.TryGetValue(key, out var value) ? value : GetRandom();
            }
        }

        public GameModeDefinition this[TanksMP.GameMode key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionaryByGameMode.TryGetValue(key, out var value) ? value : GetRandom();
            }
        }
        
        // Need to eventually take game modes into account here
        public GameModeDefinition GetRandom()
        {
            return _dictionary.ElementAt(Random.Range(0, _dictionary.Count)).Value;
        }
    }
}