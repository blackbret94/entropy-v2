using Cooldhands;
using Cooldhands.UICursor.Example;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class GamepadCursorController : MonoBehaviour
    {
        public GameObject CursorControllerGameObject;
        private bool CursorIsVisible = true;

        public void ToggleCursorState(bool cursorIsVisible)
        {
            CursorIsVisible = cursorIsVisible;
            CursorControllerGameObject.SetActive(cursorIsVisible);
        }

        public void ToggleCursorState()
        {
            ToggleCursorState(!CursorIsVisible);
        }
        
        public void SetCursorToCenter()
        {
            if(UICursorController.current != null)
            {
                UICursorController.current.CursorPosition = (new Vector2(Screen.width/2,Screen.height/2));
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Start"))
            {
                ToggleCursorState();
            }
        }
    }
}