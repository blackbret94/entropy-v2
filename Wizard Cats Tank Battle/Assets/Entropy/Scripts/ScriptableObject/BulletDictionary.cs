using System.Collections.Generic;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "BulletDictionary", menuName = "Entropy/BulletDictionary", order = 1)]
    public class BulletDictionary : UnityEngine.ScriptableObject
    {
        public Bullet[] Directory;
        private Dictionary<int, Bullet> _dictionary;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }
        
        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null) 
                return;
            
            _dictionary = new Dictionary<int, Bullet>();
         
            foreach (Bullet go in Directory)
                _dictionary.Add(go.bulletId,go);
        }

        /// <summary>
        /// Use this accessor to get the desired clip from other classes
        /// audioDirectory["shoot"]
        /// </summary>
        /// <param name="key"></param>
        public Bullet this[int key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
            }
        }
    }
}