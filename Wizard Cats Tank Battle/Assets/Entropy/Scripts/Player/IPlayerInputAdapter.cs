using UnityEngine;

namespace Entropy.Scripts.Player
{
    public interface IPlayerInputAdapter
    {
        public Vector2 GetMovementVector(out bool isMoving);
        public Vector2 GetTurretRotation(Vector3 pos);
        public bool ShouldShoot();
    }
}