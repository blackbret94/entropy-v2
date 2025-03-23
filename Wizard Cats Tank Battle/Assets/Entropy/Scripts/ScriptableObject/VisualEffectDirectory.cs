using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Visual Effect Directory", menuName = "Entropy/Visual Effect Directory", order = 1)]
    public class VisualEffectDirectory : UnityEngine.ScriptableObject
    {
        public VisualEffectData[] Directory;
        public VisualEffectData DefaultDeathFx;
        private Dictionary<string, VisualEffectData> _dictionary;
        private Dictionary<ushort, VisualEffectData> _dictionarySession;
        
        void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null || Directory == null) 
                return;
            
            _dictionary = new Dictionary<string, VisualEffectData>();
            _dictionarySession = new Dictionary<ushort, VisualEffectData>();
            _dictionary.Add("", DefaultDeathFx);

            // 0 needs to be null so that network data can be unsigned
            ushort i = 1;
            foreach (VisualEffectData visualEffect in Directory)
            {
                visualEffect.SessionId = i;
                _dictionary.Add(visualEffect.Id, visualEffect);
                _dictionarySession.Add(i, visualEffect);
                i++;
            }
        }
        
        public VisualEffectData this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
        
        public VisualEffectData GetBySessionId(ushort key)
        {
            CreateDictionaryIfDoesNotExist();
            return _dictionarySession.TryGetValue(key, out var value) ? value : null;
        }
        
        public ushort GetSessionId(VisualEffectData data)
        {
            CreateDictionaryIfDoesNotExist();
            
            return _dictionary.TryGetValue(data.Id, out var value) ? value.SessionId : (ushort)0;
        }
    }
}