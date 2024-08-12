using UnityEngine;
using UnityEngine.InputSystem;
using Vashta.Entropy.GameInput;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterGamepad : PlayerInputAdapter
    {
        public PlayerInputAdapterGamepad(InputDirectory inputDirectory, PlayerInputController playerInputController, PlayerInputActionsWCTB playerInputActionsWctb) : 
            base(inputDirectory, playerInputController, playerInputActionsWctb)
        {
        }
        
        private const string SHOOT_CODE = "Fire Controller";
        private const float MIN_TIME_BETWEEN_UI_MOVEMENTS = .2f;
        private float _lastUiMovement = 0f;
        
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

        public override bool ShouldDropCollectible()
        {
            if (PlayerInputController.GameplayActionsBlocked())
            {
                return false;
            }

            return InputDirectory.DropCollectible.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool ShouldUsePowerup()
        {
            if (PlayerInputController.GameplayActionsBlocked())
            {
                return false;
            }

            return InputDirectory.UsePowerup.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool ShouldUseUltimate()
        {
            if (PlayerInputController.GameplayActionsBlocked())
            {
                return false;
            }
            
            return InputDirectory.UseUltimate.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool ShouldToggleSettings()
        {
            return InputDirectory.OpenSettings.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool ShouldToggleClassSelection()
        {
            return InputDirectory.OpenClassSelection.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool ShouldToggleScoreboard()
        {
            return InputDirectory.OpenScoreboard.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool ShouldClosePanel()
        {
            return InputDirectory.ClosePanel.GetKeyDown(PlayerInputType.Gamepad);
        }
        
        private bool UIMovementIsValid()
        {
            return Time.time > _lastUiMovement + MIN_TIME_BETWEEN_UI_MOVEMENTS;
        }
        
        public override bool DetectUI_Up()
        {
            if (!UIMovementIsValid())
                return false;
            
            // TODO: Support arrows
            if (Input.GetAxisRaw("VerticalGamepad") >= .9999)
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
            
            // TODO: Support arrows
            if (Input.GetAxisRaw("VerticalGamepad") <= -.9999)
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
            
            // TODO: Support arrows
            if (Input.GetAxisRaw("HorizontalGamepad") <= -.9999)
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
            
            // TODO: Support arrows
            if (Input.GetAxisRaw("HorizontalGamepad") >= .9999)
            {
                _lastUiMovement = Time.time;
                return true;
            }
            
            return false;
        }

        public override bool DetectUI_Primary()
        {
            return InputDirectory.UI_Primary.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool DetectUI_Secondary()
        {
            return InputDirectory.UI_Secondary.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool DetectUI_Tertiary()
        {
            return InputDirectory.UI_Tertiary.GetKeyDown(PlayerInputType.Gamepad);
        }

        public override bool DetectUI_Quartary()
        {
            return InputDirectory.UI_Quatrary.GetKeyDown(PlayerInputType.Gamepad);
        }
    }
}