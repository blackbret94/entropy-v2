using UnityEngine;
using UnityEngine.InputSystem;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterGamepad : PlayerInputAdapter
    {
        public PlayerInputAdapterGamepad(PlayerInputController playerInputController, PlayerInputActionsWCTB playerInputActionsWctb) : 
            base(playerInputController, playerInputActionsWctb)
        {
        }
        
        public override Vector2 GetMovementVector(out bool isMoving)
        {
            InputAction iaMove = PlayerInputActions.Player.Move;
            Vector2 movementVector = iaMove.ReadValue<Vector2>();
            
            if (!PlayerInputController.GameplayActionsBlocked())
            {
                isMoving = !(movementVector.x == 0 && movementVector.y == 0);
            }
            else
            {
                isMoving = false;
            }
            
            return movementVector;
        }

        public override Vector2 GetTurretRotation(Vector3 pos)
        {
            InputAction iaAim = PlayerInputActions.Player.Aim;
            return iaAim.ReadValue<Vector2>();
        }

        public override bool ShouldShoot()
        {
            if (PlayerInputController.GameplayActionsBlocked())
            {
                return false;
            }

            return PlayerInputController.GetFireIsHeldDown();
        }
        
        public override bool DetectUI_Up()
        {
            if (!UIMovementIsValid())
                return false;

            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.y >= .9999)
            {
                _lastUiMovement = Time.time;
                return true;
            }
            
            return false;
        }

        public override bool DetectUI_Down()
        {
            if (!UIMovementIsValid())
                return false;
            
            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.y <= -.9999)
            {
                _lastUiMovement = Time.time;
                return true;
            }
            
            return false;
        }

        public override bool DetectUI_Left()
        {
            if (!UIMovementIsValid())
                return false;
            
            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.x <= -.9999)
            {
                _lastUiMovement = Time.time;
                return true;
            }
            
            return false;
        }

        public override bool DetectUI_Right()
        {
            if (!UIMovementIsValid())
                return false;
            
            InputAction iaNavigation = PlayerInputActions.UI.Navigate;
            Vector2 navVector = iaNavigation.ReadValue<Vector2>();
            
            if (navVector.x >= .9999)
            {
                _lastUiMovement = Time.time;
                return true;
            }
            
            return false;
        }
    }
}