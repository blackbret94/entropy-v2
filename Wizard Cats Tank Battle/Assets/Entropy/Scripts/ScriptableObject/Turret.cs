using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Entropy/Turret", order = 1)]
    public class Turret: ScriptableWardrobeItem
    {
        public string TurretName;
        public GameObject TurretObject;
    }
}