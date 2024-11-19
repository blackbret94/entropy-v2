using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    public class ScriptableWardrobeItem : ScriptableObjectWithID
    {
        public int Cost = 0;
        public bool AvailAtStart = false;
        public bool IsForSale = true;
        public bool InBotWardrobe = true;
        public GameObject ItemObject;
        public string ItemName;
        public string ItemDescription;
        public Sprite Icon;
        public Rarity Rarity;
    }
}