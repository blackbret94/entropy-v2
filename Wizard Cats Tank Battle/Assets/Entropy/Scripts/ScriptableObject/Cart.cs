using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "BodyType", menuName = "Entropy/Cart", order = 1)]
    public class Cart: UnityEngine.ScriptableObject
    {
        public int CartId;
        public string CartName;
        public GameObject CartObject;
    }
}