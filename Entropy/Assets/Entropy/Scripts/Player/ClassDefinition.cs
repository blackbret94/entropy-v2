using UnityEngine;

namespace Entropy.Scripts.Player
{
    [CreateAssetMenu(fileName = "Class Definition", menuName = "Entropy/Class Definition", order = 1)]
    public class ClassDefinition: ScriptableObject
    {
        public int classId = 0;
        public string className = "DEFAULT";
        public int maxHealth = 10;
        public float fireRate = 0.75f;
        public float moveSpeed = 8f;
        public int damageAmtOnCollision = 5;
        public int armor = 2;
    }
}