using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Cart", menuName = "Entropy/Cart", order = 1)]
    public class Cart: ScriptableWardrobeItem
    {
        public override WardrobeCategory Category => WardrobeCategory.CART;
    }
}