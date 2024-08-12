using UnityEngine;

namespace Vashta.Entropy.GameInput
{
    [CreateAssetMenu(fileName = "Keycode Sprite", menuName = "Entropy/Keycode Sprite", order = 1)]
    public class KeycodeSprite : UnityEngine.ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Sprite;
        public KeyCode KeyCode;
    }
}