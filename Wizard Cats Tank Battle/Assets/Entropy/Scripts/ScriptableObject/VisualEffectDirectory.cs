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
        
        void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null || Directory == null) 
                return;
            
            _dictionary = new Dictionary<string, VisualEffectData>();
            _dictionary.Add("", DefaultDeathFx);
         
            foreach (VisualEffectData visualEffect in Directory)
                _dictionary.Add(visualEffect.Id,visualEffect);
        }
        
        public VisualEffectData this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
    }
}