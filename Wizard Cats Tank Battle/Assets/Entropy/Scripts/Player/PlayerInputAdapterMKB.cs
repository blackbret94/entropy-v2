using UnityEngine;
using UnityEngine.EventSystems;
using Vashta.Entropy.SaveLoad;

namespace Entropy.Scripts.Player
{
    public class PlayerInputAdapterMKB : PlayerInputAdapter
    {
        private const string SHOOT_CODE = "Fire1";
        
        public PlayerInputAdapterMKB(InputDirectory inputDirectory, PlayerInputController playerInputController) : base(inputDirectory, playerInputController)
        {
        }
        
        public override Vector2 GetMovementVector(out bool isMoving)
        {
            isMoving = !(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0);
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
            return Input.GetButton(SHOOT_CODE) && !EventSystem.current.IsPointerOverGameObject();
        }

        public override bool ShouldDropCollectible()
        {
            return InputDirectory.DropCollectible.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool ShouldUsePowerup()
        {
            return InputDirectory.UsePowerup.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool ShouldUseUltimate()
        {
            return InputDirectory.UseUltimate.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool ShouldToggleSettings()
        {
            return InputDirectory.OpenSettings.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool ShouldToggleClassSelection()
        {
            return InputDirectory.OpenClassSelection.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool ShouldToggleScoreboard()
        {
            return InputDirectory.OpenScoreboard.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool ShouldClosePanel()
        {
            return InputDirectory.ClosePanel.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Up()
        {
            return InputDirectory.UI_Up.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Down()
        {
            return InputDirectory.UI_Down.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Left()
        {
            return InputDirectory.UI_Left.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Right()
        {
            return InputDirectory.UI_Right.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Primary()
        {
            return InputDirectory.UI_Primary.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Secondary()
        {
            return InputDirectory.UI_Secondary.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Tertiary()
        {
            return InputDirectory.UI_Tertiary.GetKeyDown(PlayerInputType.MKb);
        }

        public override bool DetectUI_Quartary()
        {
            return InputDirectory.UI_Quatrary.GetKeyDown(PlayerInputType.MKb);
        }
    }
}