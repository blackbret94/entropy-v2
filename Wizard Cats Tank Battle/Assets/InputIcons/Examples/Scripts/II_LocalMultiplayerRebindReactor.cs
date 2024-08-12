using InputIcons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class II_LocalMultiplayerRebindReactor : MonoBehaviour
{

    public InputActionAsset originalInputActionAsset;

    public PlayerInput playerInputToUpdate;


    void Start()
    {
        if(playerInputToUpdate == null)
            playerInputToUpdate = gameObject.AddComponent<PlayerInput>();

        InputIconsManagerSO.onBindingsChanged += CopyOverridenBindingsToActionAsset;
        InputIconsManagerSO.onNewBindingsSaved += CopyOverridenBindingsToActionAsset;
        InputIconsManagerSO.onBindingsReset += ResetBindings;
        CopyOverridenBindingsToActionAsset();
    }

    private void OnDestroy()
    {
        InputIconsManagerSO.onBindingsChanged -= CopyOverridenBindingsToActionAsset;
        InputIconsManagerSO.onNewBindingsSaved -= CopyOverridenBindingsToActionAsset;
        InputIconsManagerSO.onBindingsReset -= ResetBindings;
    }

    public void CopyOverridenBindingsToActionAsset()
    {
        ResetBindings();
        Dictionary<string, string> overrides = InputIconsManagerSO.GetSavedBindingOverrides();

        //walk through action maps check dictionary for overrides
     
        foreach (InputActionMap map in playerInputToUpdate.actions.actionMaps)
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

    private void ResetBindings()
    {
        playerInputToUpdate.actions.RemoveAllBindingOverrides();
        //UpdatePlayerInputBindings();
    }
}
