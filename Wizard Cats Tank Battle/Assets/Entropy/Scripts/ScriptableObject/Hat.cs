using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Hat", menuName = "Entropy/Hat", order=1)]
    public class Hat: ScriptableWardrobeItem
    {
        public override WardrobeCategory Category => WardrobeCategory.HAT;
    }
}