using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Visual Effect Directory", menuName = "Entropy/Visual Effect Directory", order = 1)]
    public class VisualEffectDirectory : UnityEngine.ScriptableObject
    {
        public VisualEffect[] Directory;
        public VisualEffect DefaultDeathFx;
        private Dictionary<string, VisualEffect> _dictionary;
        
        void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null || Directory == null) 
                return;
            
            _dictionary = new Dictionary<string, VisualEffect>();
            _dictionary.Add("", DefaultDeathFx);
         
            foreach (VisualEffect visualEffect in Directory)
                _dictionary.Add(visualEffect.Id,visualEffect);
        }
        
        public VisualEffect this[string key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
    }
}