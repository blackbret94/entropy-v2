using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.StatusEffects
{
    [CreateAssetMenu(fileName = "StatusEffectDirectory", menuName = "Entropy/StatusEffectDirectory", order = 1)]
    public class StatusEffectDirectory : UnityEngine.ScriptableObject
    {
        public StatusEffectData[] Directory;
        private Dictionary<string, StatusEffectData> _dictionary;
        
        void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }

        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            _dictionary = new Dictionary<string, StatusEffectData>();
         
            foreach (StatusEffectData statusEffect in Directory)
                _dictionary.Add(statusEffect.Id,statusEffect);
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// audioDirectory["shoot"]
        /// </summary>
        /// <param name="key"></param>
        public StatusEffectData this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
    }
}