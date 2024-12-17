using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class JoystickController : MonoBehaviour
    {
        public UIJoystick MovementJoystick; // Left by default
        public UIJoystick ShootJoystick; // Right by default
        
        // Need to seperate out player movement and aim controls before this is worth implementing

        public void SimulateInput(Vector2 moveDir, Vector2 aimDir)
        {
            MovementJoystick.position = moveDir;
            ShootJoystick.position = aimDir;
        }

        public void ResetInput()
        {
            MovementJoystick.OnEndDrag(null);
            ShootJoystick.OnEndDrag(null);
        }
    }
}