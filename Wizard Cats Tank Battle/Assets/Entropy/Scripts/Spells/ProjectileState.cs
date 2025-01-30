using FusionHelpers;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Spells
{
    // This probably doesn't take bounce into account!
    // TODO: On bounce detection, this needs to update start position and direction
    // TODO: New struct needs to be created for changes in speed
    public struct ProjectileState : ISparseState<Projectile>
    {
        /// <summary>
        /// Generic sparse state properties required by the interface
        /// </summary>
        public int StartTick { get; set; }
        public int EndTick { get; set; }

        /// <summary>
        /// Shot specific sparse properties
        /// </summary>
        public Vector3 Position;
        public Vector3 Direction;
        public int ClassId;
        public float DamageModifier;

        public ProjectileState(Vector3 startPosition, Vector3 direction, int classId, float damageModifier=0f)
        {
            StartTick = 0;
            EndTick = 0;
            Position = startPosition;
            Direction = direction;
            ClassId = classId;
            DamageModifier = damageModifier;
        }

        public void Extrapolate(float t, Projectile prefab)
        {
            Position = GetPositionAt(t, prefab);
            Direction = GetDirectionAt(t, prefab);
        }

        public Vector3 GetTargetPosition(Projectile prefab)
        {
            float a = 0.5f * prefab.Gravity.y;
            float b = prefab.Speed * Direction.y;
            float c = Position.y;
            float d = b * b - 4 * a * c;
            float t = (-b - Mathf.Sqrt(d))/(2*a);
            Vector3 p = GetPositionAt(t, prefab);
            p.y = 0.05f; // Return the position with a slight y offset to avoid placing target where it will end up z-fighting with the ground;
            return p;
        }

        private Vector3 GetPositionAt(float t, Projectile prefab) => Position + t * (prefab.Speed * Direction + 0.5f * t * prefab.Gravity);
        private Vector3 GetDirectionAt(float t, Projectile prefab) => prefab.Speed==0 ? Direction : (prefab.Speed * Direction + t * prefab.Gravity).normalized;
    }
}