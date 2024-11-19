using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    public enum Rarity
    {
        Basic,
        Uncommon,
        Rare,
        Legendary
    }

    [CreateAssetMenu(fileName = "Rarity Definition", menuName = "Entropy/Rarity", order = 1)]
    public class RarityDefinition : UnityEngine.ScriptableObject
    {
        public string Id;
        public string Name;
        public Rarity Rarity;
        public Color Color;
        public Sprite BackgroundImage;
    }
}