using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Meow", menuName = "Entropy/Meow", order = 1)]
    public class Meow: ScriptableWardrobeItem
    {
        public AudioClip AudioClip;
    }
}