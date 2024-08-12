using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Vashta.Entropy.GameInput;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterMKB : PlayerInputAdapter
    {
        public PlayerInputAdapterMKB(InputDirectory inputDirectory, PlayerInputController playerInputController, PlayerInputActionsWCTB playerInputActions) : 
            base(inputDirectory, playerInputController, playerInputActions)
        {
        }
        
        // public override Vector2 GetMovementVector(out bool isMoving)
        // {
        //     isMoving = !(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0);
        //     return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // }
        
        public override Vector2 GetMovementVector(out bool isMoving)
        {
            InputAction iaMove = PlayerInputActions.Player.Move;
            Vector2 movementVector = iaMove.ReadValue<Vector2>();
            isMoving = !(movementVector.x == 0 && movementVector.y == 0);
            
            return movementVector;
        }

        public override Vector2 GetTurretRotation(Vector3 pos)
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

        public override bool ShouldShoot()
        {
            return PlayerInputController.GetFireIsHeldDown() && !EventSystem.current.IsPointerOverGameObject();
        }

        public override bool DetectUI_Up()
        {
            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.y >= .9999)
            {
                return true;
            }
            
            return false;
        }

        public override bool DetectUI_Down()
        {
            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.y <= -.9999)
            {
                return true;
            }
            
            return false;
        }

        public override bool DetectUI_Left()
        {
            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.x <= -.9999)
            {
                return true;
            }
            
            return false;
        }

        public override bool DetectUI_Right()
        {
            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.x >= .9999)
            {
                return true;
            }
            
            return false;
        }
    }
}