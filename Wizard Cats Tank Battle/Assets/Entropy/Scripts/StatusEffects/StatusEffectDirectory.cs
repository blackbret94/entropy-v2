using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.StatusEffects
{
    [CreateAssetMenu(fileName = "StatusEffectDirectory", menuName = "Entropy/StatusEffectDirectory", order = 1)]
    public class StatusEffectDirectory : UnityEngine.ScriptableObject
    {
        public StatusEffectData[] Directory;
        private Dictionary<string, StatusEffectData> _dictionary;
        private Dictionary<int, StatusEffectData> _dictionarySession;
        
        void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }

        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            _dictionary = new Dictionary<string, StatusEffectData>();
            _dictionarySession = new Dictionary<int, StatusEffectData>();

            // 0 needs to be null so that network data can be unsigned
            ushort i = 1;
            foreach (StatusEffectData statusEffect in Directory)
            {
                statusEffect.SessionId = i;
                _dictionary.Add(statusEffect.Id,statusEffect);
                _dictionarySession.Add(i, statusEffect);
                i++;
            }
        }

        public StatusEffectData this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.TryGetValue(key, out var value) ? value : null;
            }
        }

        public StatusEffectData GetBySessionId(ushort key)
        {
            CreateDictionaryIfDoesNotExist();
            return _dictionarySession.TryGetValue(key, out var value) ? value : null;
        }

        public ushort GetSessionId(StatusEffectData data)
        {
            CreateDictionaryIfDoesNotExist();
            
            return _dictionary.TryGetValue(data.Id, out var value) ? value.SessionId : (ushort)0;
        }
    }
}