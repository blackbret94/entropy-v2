using Entropy.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Vashta.Entropy.GameInput
{
    
    [CreateAssetMenu(fileName = "Input Code", menuName = "Entropy/Input Code", order = 1)]
    public class InputCode : UnityEngine.ScriptableObject
    {
        public string Name;
        public string Description;
        
        public string KeyMKB;
        public KeyCode DefaultValueMKB;
        public string KeyGamePad;
        public KeyCode DefaultValueGamepad;

        // New input system!
        // public InputAction InputAction;

        public KeyCode GetValue(PlayerInputType playerInputType)
        {
            if(playerInputType == PlayerInputType.MKb)
                return (KeyCode)int.Parse(PlayerPrefs.GetString(KeyMKB, ((int)DefaultValueMKB).ToString()));
            
            if(playerInputType == PlayerInputType.Gamepad)
                return (KeyCode)int.Parse(PlayerPrefs.GetString(KeyGamePad, ((int)DefaultValueGamepad).ToString()));

            return KeyCode.None;
        }

        public void SetValue(KeyCode value, PlayerInputType playerInputType)
        {
            if(playerInputType == PlayerInputType.MKb)
                PlayerPrefs.SetString(KeyMKB, ((int)value).ToString());
            
            if(playerInputType == PlayerInputType.Gamepad)
                PlayerPrefs.SetString(KeyGamePad, ((int)value).ToString());
        }

        public bool GetKeyDown(PlayerInputType playerInputType)
        {
            return Input.GetKeyDown(GetValue(playerInputType));
        }

        public bool GetKey(PlayerInputType playerInputType)
        {
            return Input.GetKey(GetValue(playerInputType));
        }

        public bool GetKeyUp(PlayerInputType playerInputType)
        {
            return Input.GetKeyUp(GetValue(playerInputType));
        }
    }
}