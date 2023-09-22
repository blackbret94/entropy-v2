using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooldhands;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Cooldhands.UICursor.Example
{
    public class ButtonActions : MonoBehaviour
    {
        // Update is called once per frame
        public void SetCursorToCenter()
        {
            if(UICursorController.current != null)
            {
                UICursorController.current.CursorPosition = (new Vector2(Screen.width/2,Screen.height/2));
            }
        }

        public void Log(string value)
        {
            Debug.Log(value);
        }

        public void SetNavigation(bool value)
        {
            EventSystem.current.sendNavigationEvents = value;
        }

        public void Pause(bool value)
        {
            if(value)
            {
                Time.timeScale = 0.0f;
            }
            else{
                Time.timeScale = 1.0f;
            }
        }
    }
}
