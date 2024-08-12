using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.GameInput
{
    [CreateAssetMenu(fileName = "Keycode Sprite Dictionary", menuName = "Entropy/Keycode Sprite Dictionary", order = 1)]
    public class KeycodeSpriteDictionary : UnityEngine.ScriptableObject
    {
        public KeycodeSprite[] Directory;
        private Dictionary<KeyCode, KeycodeSprite> _dictionary;

        private void OnEnable()
        {
            CreateDictionaryIfDoesNotExist();
        }

        private void CreateDictionaryIfDoesNotExist()
        {
            if (_dictionary != null)
                return;

            _dictionary = new Dictionary<KeyCode, KeycodeSprite>();

            foreach (var keycodeSprite in Directory)
            {
                _dictionary.Add(keycodeSprite.KeyCode, keycodeSprite);
            }

        }

        public KeycodeSprite this[KeyCode key]
        {
            get
            {
                CreateDictionaryIfDoesNotExist();
                return _dictionary.TryGetValue(key, out var value) ? value : null;
            }
        }
    }
}