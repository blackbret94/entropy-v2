using UnityEngine;
using Vashta.Entropy.SaveLoad;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterGamepad : PlayerInputAdapter
    {
        public PlayerInputAdapterGamepad(InputDirectory inputDirectory, PlayerInputController playerInputController) : base(inputDirectory, playerInputController)
        {
        }
        
        private const string SHOOT_CODE = "Fire Controller";
        private const float MIN_TIME_BETWEEN_UI_MOVEMENTS = .2f;
        private float _lastUiMovement = 0f;
        
        public override Vector2 GetMovementVector(out bool isMoving)
        {
            if (!PlayerInputController.GameplayActionsBlocked())
            {
                isMoving = !(Input.GetAxisRaw("HorizontalGamepad") == 0 && Input.GetAxisRaw("VerticalGamepad") == 0);
            }
            else
            {
                isMoving = false;
            }
            
            return new Vector2(Input.GetAxis("HorizontalGamepad"), Input.GetAxis("VerticalGamepad"));
        }

        public override Vector2 GetTurretRotation(Vector3 pos)
        {
            return new Vector2(Input.GetAxis("Rotate X"), Input.GetAxis("Rotate Y"));
        }

        public override bool ShouldShoot()
        {
            if (PlayerInputController.GameplayActionsBlocked())
            {
                return false;
            }

            return Input.GetAxisRaw(SHOOT_CODE) > .001;
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