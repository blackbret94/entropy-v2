using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterTouch : IPlayerInputAdapter
    {

        public Vector2 GetMovementVector(out bool isMoving)
        {
            throw new System.NotImplementedException();
        }

        public Vector2 GetTurretRotation(Vector3 pos)
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldShoot()
        {
            return false;
        }
    }
}