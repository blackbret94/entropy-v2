using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Entropy/Turret", order = 1)]
    public class Turret: ScriptableWardrobeItem
    {
        public int TurretId;
        public string TurretName;
        public GameObject TurretObject;
    }
}