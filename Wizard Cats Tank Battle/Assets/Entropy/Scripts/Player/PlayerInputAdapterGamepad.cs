using UnityEngine;
using UnityEngine.EventSystems;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterGamepad : IPlayerInputAdapter
    {
        private const string SHOOT_CODE = "Fire Controller";
        
        public Vector2 GetMovementVector(out bool isMoving)
        {
            isMoving = !(Input.GetAxisRaw("HorizontalGamepad") == 0 && Input.GetAxisRaw("VerticalGamepad") == 0);
            return new Vector2(Input.GetAxis("HorizontalGamepad"), Input.GetAxis("VerticalGamepad"));
        }

        public Vector2 GetTurretRotation(Vector3 pos)
        {
            return new Vector2(Input.GetAxis("Rotate X"), Input.GetAxis("Rotate Y"));
        }

        public bool ShouldShoot()
        {
            return Input.GetAxisRaw(SHOOT_CODE) > .001;
        }
    }
}