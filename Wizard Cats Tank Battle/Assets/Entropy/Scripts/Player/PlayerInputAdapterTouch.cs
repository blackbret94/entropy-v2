using UnityEngine;
using Vashta.Entropy.GameInput;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterTouch : PlayerInputAdapter
    {
        public PlayerInputAdapterTouch(InputDirectory inputDirectory, PlayerInputController playerInputController, PlayerInputActionsWCTB playerInputActionsWctb) : base(inputDirectory, playerInputController, playerInputActionsWctb)
        {
        }
        
        public override Vector2 GetMovementVector(out bool isMoving)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2 GetTurretRotation(Vector3 pos)
        {
            throw new System.NotImplementedException();
        }

        public override bool ShouldShoot()
        {
            return false;
        }

        public override bool ShouldDropCollectible()
        {
            return false;
        }

        public override bool ShouldUsePowerup()
        {
            return false;
        }

        public override bool ShouldUseUltimate()
        {
            return false;
        }

        public override bool ShouldToggleSettings()
        {
            return false;
        }

        public override bool ShouldToggleClassSelection()
        {
            return false;
        }

        public override bool ShouldToggleScoreboard()
        {
            return false;
        }

        public override bool ShouldClosePanel()
        {
            return false;
        }

        public override bool DetectUI_Up()
        {
            return false;
        }

        public override bool DetectUI_Down()
        {
            return false;
        }

        public override bool DetectUI_Left()
        {
            return false;
        }

        public override bool DetectUI_Right()
        {
            return false;
        }

        public override bool DetectUI_Primary()
        {
            return false;
        }

        public override bool DetectUI_Secondary()
        {
            return false;
        }

        public override bool DetectUI_Tertiary()
        {
            return false;
        }

        public override bool DetectUI_Quartary()
        {
            return false;
        }
    }
}