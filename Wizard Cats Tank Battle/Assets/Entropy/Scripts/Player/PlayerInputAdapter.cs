using UnityEngine;
using UnityEngine.InputSystem;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.Player
{
    public abstract class PlayerInputAdapter
    {
        protected readonly PlayerInputController PlayerInputController;
        protected readonly PlayerInputActionsWCTB PlayerInputActions;
        
        protected const float MIN_TIME_BETWEEN_UI_MOVEMENTS = .2f;
        protected float _lastUiMovement = 0f;

        public PlayerInputAdapter(PlayerInputController playerInputController, 
            PlayerInputActionsWCTB playerInputActions)
        {
            PlayerInputController = playerInputController;
            PlayerInputActions = playerInputActions;
        }

        public virtual void Update()
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
        
        // Zoom
        public Vector2 GetZoomVector()
        {
            InputAction iaZoom = PlayerInputActions.UI.Zoom;
            return iaZoom.ReadValue<Vector2>();
        }
        
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
        
        protected bool UIMovementIsValid()
        {
            return Time.time > _lastUiMovement + MIN_TIME_BETWEEN_UI_MOVEMENTS;
        }
    }
}