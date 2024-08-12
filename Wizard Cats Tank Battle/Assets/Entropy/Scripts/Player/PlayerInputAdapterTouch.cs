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
    }
}