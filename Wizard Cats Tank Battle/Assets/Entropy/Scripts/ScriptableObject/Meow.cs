using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Meow", menuName = "Entropy/Meow", order = 1)]
    public class Meow: ScriptableWardrobeItem
    {
        public AudioClip AudioClip;
        public override WardrobeCategory Category => WardrobeCategory.MEOW;

    }
}