using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "PowerupDirectory", menuName = "Entropy/PowerupDirectory", order = 1)]
    public class PowerupDirectory: UnityEngine.ScriptableObject
    {
        public Powerup[] Directory;
        private Dictionary<int, Powerup> _dictionary;
        
        void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }

        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            _dictionary = new Dictionary<int, Powerup>();
        
            foreach (Powerup powerup in Directory)
                _dictionary.Add(powerup.PowerupId,powerup);
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// audioDirectory["shoot"]
        /// </summary>
        /// <param name="key"></param>
        public Powerup this[int key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
    }
}