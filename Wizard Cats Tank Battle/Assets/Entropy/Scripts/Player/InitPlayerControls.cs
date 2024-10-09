using System.Collections.Generic;
using InputIcons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entropy.Scripts.Player
{
    
    public class InitPlayerControls : MonoBehaviour
    {
        public static PlayerInputActionsWCTB PlayerInputActions;
        
        private void Awake()
        {
            if (PlayerInputActions == null)
            {
                DontDestroyOnLoad(gameObject);
                
                PlayerInputActions = new PlayerInputActionsWCTB();

                if (InputIconsManagerSO.Instance.loadAndSaveInputBindingOverrides)
                {
                    LoadSavedBindingOverrides();

                    InputIconsManagerSO.onNewBindingsSaved += LoadSavedBindingOverrides;
                }

                InputIconsManagerSO.onBindingsReset += HandleAllBindingsReset;
                PlayerInputActions.Player.Enable();
                PlayerInputActions.UI.Enable();
            }
        }
        
        //1. Removes any overrides
        //2. Loads the saved binding overrides of the used Input Action Assets (saved in PlayerPrefs)
        //3. Overrides the bindings of the generated C# class with the saved bindings
        private void LoadSavedBindingOverrides()
        {
            PlayerInputActions.asset.RemoveAllBindingOverrides();

            Dictionary<string, string> overrides = InputIconsManagerSO.GetSavedBindingOverrides();

            //walk through action maps check dictionary for overrides
            foreach (InputActionMap map in PlayerInputActions.asset.actionMaps)
            {
                var bindings = map.bindings;
                for (int i = 0; i < bindings.Count; ++i)
                {
                    if (overrides.TryGetValue((map.name + "/" + bindings[i].id), out string overridePath))
                    {
                        //if there is an override apply it
                        map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                    }
                }
            }
        }

        //When the player resets the bindings of the used Input Action Assets, also reset the bindings of the generated C# class
        private void HandleAllBindingsReset()
        {
            PlayerInputActions.asset.RemoveAllBindingOverrides();
        }
    }
}