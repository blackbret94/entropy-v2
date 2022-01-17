using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Entropy/Turret", order = 1)]
    public class Turret: UnityEngine.ScriptableObject
    {
        public int TurretId;
        public string TurretName;
        public GameObject TurretObject;
    }
}