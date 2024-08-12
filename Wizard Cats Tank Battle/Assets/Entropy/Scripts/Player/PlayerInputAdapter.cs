using UnityEngine;
using Vashta.Entropy.GameInput;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.Player
{
    public abstract class PlayerInputAdapter
    {
        protected readonly InputDirectory InputDirectory;
        protected readonly PlayerInputController PlayerInputController;
        protected readonly PlayerInputActionsWCTB PlayerInputActions;

        public PlayerInputAdapter(InputDirectory inputDirectory, PlayerInputController playerInputController, 
            PlayerInputActionsWCTB playerInputActions)
        {
            InputDirectory = inputDirectory;
            PlayerInputController = playerInputController;
            PlayerInputActions = playerInputActions;
        }

        public void Update()
        {
            // UI Directions
            if(DetectUI_Up()) UI_Up();
            if(DetectUI_Down()) UI_Down();
            if(DetectUI_Left()) UI_Left();
            if(DetectUI_Right()) UI_Right();
        }
        
        // Gameplay
        public abstract Vector2 GetMovementVector(out bool isMoving);

        public abstract Vector2 GetTurretRotation(Vector3 pos);
        
        public abstract bool ShouldShoot();
        
        // UI Directions
        public abstract bool DetectUI_Up();
        public abstract bool DetectUI_Down();
        public abstract bool DetectUI_Left();
        public abstract bool DetectUI_Right();
        
        // UI Actions
        protected void UI_Up()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Up();
            }
        }

        protected void UI_Down()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Down();
            }
        }

        protected void UI_Left()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Left();
            }
        }

        protected void UI_Right()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Right();
            }
        }
    }
}