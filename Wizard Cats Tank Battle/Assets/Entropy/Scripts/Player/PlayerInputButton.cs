using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Entropy.Scripts.Player
{
    public class PlayerInputButton
    {
        public bool IsDown { get; private set; }

        public virtual void OnPress(InputAction.CallbackContext context)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                IsDown = true;
            }
        }

        public virtual void OnRelease(InputAction.CallbackContext context)
        {
            IsDown = false;
        }
    }
}