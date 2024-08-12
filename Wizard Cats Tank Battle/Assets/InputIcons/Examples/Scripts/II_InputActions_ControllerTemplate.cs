using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputIcons
{

    //If you want to use a generated C# class and the Rebinding Prefabs, use some of the code below in your
    //own classes to keep the binding overrides in the Input Action Assets and in the instances of the
    //generated C# class in sync.
    //The class subscribes to events (onNewBindingsSaved and onBindingsReset) and overrides the bindings of the
    //generated C# class when necessary
    public class II_InputActions_ControllerTemplate : MonoBehaviour
    {
        //This is the generated C# class which the Input Action Asset generates.
        //Swap 'II_InputActions_Player' with your generated class
        public static II_InputActions_Player inputActions;

        //a reference to the player object we want to control, swap it with your own player class or whatever you use
        public II_Player player; 

        private void Awake()
        {
            if (inputActions == null)
            {
                inputActions = new II_InputActions_Player();

                //Load the saved overrides into the generated class on Awake to sync up with the Input Action Asset
                if (InputIconsManagerSO.Instance.loadAndSaveInputBindingOverrides)
                {
                    LoadSavedBindingOverrides();

                    //Subscribe to the onNewBindingsSaved event on the manager and override the bindings of the generated C# class
                    InputIconsManagerSO.onNewBindingsSaved += LoadSavedBindingOverrides;
                }

                //Subscribe to the onBindingsReset event to also reset the bindings of the generated class when all bindings of the Input Action Asset get reset
                InputIconsManagerSO.onBindingsReset += HandleAllBindingsReset;


                inputActions.PlatformerControls.Enable();
            }
        }


        //1. Removes any overrides
        //2. Loads the saved binding overrides of the used Input Action Assets (saved in PlayerPrefs)
        //3. Overrides the bindings of the generated C# class with the saved bindings
        private void LoadSavedBindingOverrides()
        {
            inputActions.asset.RemoveAllBindingOverrides();

            Dictionary<string, string> overrides = InputIconsManagerSO.GetSavedBindingOverrides();

            //walk through action maps check dictionary for overrides
            foreach (InputActionMap map in inputActions.asset.actionMaps)
            {
                var bindings = map.bindings;
                for (int i = 0; i < bindings.Count; ++i)
                {
                    if (overrides.TryGetValue((map.name + "/" + bindings[i].id).ToString(), out string overridePath))
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
            inputActions.asset.RemoveAllBindingOverrides();
        }







        #region ... Code For Movement (example code) 
        //listen for necessary events
        private void OnEnable()
        {
            inputActions.PlatformerControls.Move.performed += OnMovement;
            inputActions.PlatformerControls.Attack.performed += On_Plattformer_Attack;
            inputActions.PlatformerControls.Jump.performed += OnJump;
            inputActions.PlatformerControls.Pause.performed += OnTogglePause;
            inputActions.PlatformerControls.Submit.performed += OnSubmit;
            inputActions.PlatformerControls.Cancel.performed += OnCancel;
            inputActions.PlatformerControls.SwitchWeapon.performed += OnWeaponSwitch;

            inputActions.HelicopterControls.Move.performed += OnHelicopterMovement;
            inputActions.HelicopterControls.ThrowBomb.performed += OnHelicopterThrowBomb;
            inputActions.HelicopterControls.Rotate.performed += OnHelicopterRotate;
        }


        //dont forget to stop listening for events
        private void OnDisable()
        {
            inputActions.PlatformerControls.Move.performed -= OnMovement;
            inputActions.PlatformerControls.Attack.performed -= On_Plattformer_Attack;
            inputActions.PlatformerControls.Jump.performed -= OnJump;
            inputActions.PlatformerControls.Pause.performed -= OnTogglePause;
            inputActions.PlatformerControls.Submit.performed -= OnSubmit;
            inputActions.PlatformerControls.Cancel.performed -= OnCancel;
            inputActions.PlatformerControls.SwitchWeapon.performed -= OnWeaponSwitch;

            inputActions.HelicopterControls.Move.performed -= OnHelicopterMovement;
            inputActions.HelicopterControls.ThrowBomb.performed -= OnHelicopterThrowBomb;
            inputActions.HelicopterControls.Rotate.performed -= OnHelicopterRotate;
        }


        //Handle various inputs down below
        //...
        public void SwitchControls()
        {
            if (inputActions.PlatformerControls.enabled)
                ActivateHelicopterControls();
            else
                ActivatePlatformerControls();
        }

        public void ActivatePlatformerControls()
        {
            inputActions.PlatformerControls.Enable();
        }

        public void ActivateHelicopterControls()
        {
            inputActions.HelicopterControls.Enable();
        }

        public void On_Plattformer_Attack(InputAction.CallbackContext context)
        {
            player.OnAttack(context);
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            player.OnMovement(value);
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            player.OnJump(value);
        }

        public void OnTogglePause(InputAction.CallbackContext value)
        {
            player.OnTogglePause(value);
        }

        public void OnSubmit(InputAction.CallbackContext value)
        {
            player.OnSubmit(value);
        }

        public void OnCancel(InputAction.CallbackContext value)
        {
            player.OnCancel(value);
        }


        public void OnWeaponSwitch(InputAction.CallbackContext value)
        {
            player.OnWeaponSwitch(value);
        }


        private void OnHelicopterRotate(InputAction.CallbackContext obj)
        {
            Debug.Log("helicopter rotate");
        }

        private void OnHelicopterThrowBomb(InputAction.CallbackContext obj)
        {
            Debug.Log("helicopter throw bomb");
        }

        private void OnHelicopterMovement(InputAction.CallbackContext obj)
        {
            Debug.Log("helicopter move");
        }
        #endregion 
    }
}

