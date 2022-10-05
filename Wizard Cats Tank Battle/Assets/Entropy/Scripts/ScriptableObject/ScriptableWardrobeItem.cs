using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    public class ScriptableWardrobeItem : ScriptableObjectWithID
    {
        public int Cost = 0;
        public bool AvailAtStart = false;
        public GameObject ItemObject;
        public string ItemName;
    }
}