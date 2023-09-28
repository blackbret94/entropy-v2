using UnityEngine;
using UnityEngine.EventSystems;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterMKB : IPlayerInputAdapter
    {
        private const string SHOOT_CODE = "Fire1";
        
        public Vector2 GetMovementVector(out bool isMoving)
        {
            isMoving = !(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0);
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        public Vector2 GetTurretRotation(Vector3 pos)
        {
            //cast a ray on a plane at the mouse position for detecting where to shoot 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up);
            float distance = 0f;
            Vector3 hitPos = Vector3.zero;
            //the hit position determines the mouse position in the scene
            if (plane.Raycast(ray, out distance))
            {
                hitPos = ray.GetPoint(distance) - pos;
            }

            //we've converted the mouse position to a direction
            return new Vector2(hitPos.x, hitPos.z);
        }

        public bool ShouldShoot()
        {
            return Input.GetButton(SHOOT_CODE) && !EventSystem.current.IsPointerOverGameObject();
        }
    }
}