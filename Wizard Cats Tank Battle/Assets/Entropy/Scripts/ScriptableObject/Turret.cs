using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Entropy/Turret", order = 1)]
    public class Turret: ScriptableWardrobeItem
    {
        public override WardrobeCategory Category => WardrobeCategory.TURRET;
    }
}