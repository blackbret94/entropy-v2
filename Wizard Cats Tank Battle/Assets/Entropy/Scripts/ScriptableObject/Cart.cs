using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "BodyType", menuName = "Entropy/Cart", order = 1)]
    public class Cart: ScriptableWardrobeItem
    {
        public string CartName;
        public GameObject CartObject;
    }
}