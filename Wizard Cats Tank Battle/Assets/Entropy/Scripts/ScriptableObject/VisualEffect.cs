using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "VisualEffect", menuName = "Entropy/Visual Effect", order = 1)]
    public class VisualEffect : ScriptableObjectWithID
    {
        public GameObject VisualEffectPrefab;
    }
}