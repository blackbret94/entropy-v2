using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Projectile Dictionary", menuName = "Entropy/Projectile Dictionary", order = 1)]
    public class ProjectileDirectory : UnityEngine.ScriptableObject
    {
        public ProjectileData[] Directory;
        private Dictionary<int, ProjectileData> _dictionary;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            // Create stringId indexed dictionary
            _dictionary = new Dictionary<int, ProjectileData>();
            
            foreach (ProjectileData go in Directory)
            {
                _dictionary.Add(go.ProjectileId, go);
            }
        }

        public ProjectileData this[int key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.TryGetValue(key, out var value) ? value : GetRandom();
            }
        }
        
        public ProjectileData GetRandom()
        {
            return _dictionary.ElementAt(Random.Range(0, _dictionary.Count)).Value;
        }
    }
}