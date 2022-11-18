using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Powerup", menuName = "Entropy/Powerup", order = 1)]
    public class Powerup : ScriptableObjectWithID
    {
        public int PowerupId;
        public string PowerupName;
        public Sprite Icon;
        public Color Color;
        public float MaxValue;
        public string DisplayText;
        public string DisplaySubtext;
    }
}