using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "VisualEffect", menuName = "Entropy/Visual Effect", order = 1)]
    public class VisualEffect : ScriptableObjectWithID
    {
        public GameObject VisualEffectPrefab;
        public Slot slot;
        public bool loops;
        public float duration;
    }
}