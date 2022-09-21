using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Hat", menuName = "Entropy/Hat", order=1)]
    public class Hat: ScriptableWardrobeItem
    {
        public int HatId;
        public string HatName;
        public GameObject HatObject;
    }
}