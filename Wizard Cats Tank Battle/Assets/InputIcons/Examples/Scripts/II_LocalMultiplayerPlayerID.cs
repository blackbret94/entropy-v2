using InputIcons;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class II_LocalMultiplayerPlayerID : MonoBehaviour
{
    private static int nextID = 0;
    public bool useAutomaticID = true;
    public int playerID = 0;
    public string usedControlScheme = "";
    public string controlSchemeNameKeyboard = "Keyboard and Mouse";
    public string controlSchemeNameGamepad = "Gamepad";

    public GameObject playerPrompts;

    private void Awake()
    {
        if (playerPrompts)
            playerPrompts.SetActive(false);

        if (useAutomaticID)
        {
            playerID = nextID;
            nextID++;
        }
    }


    private void Start()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        if (input)
        {
            InputDevice myDevice = input.devices[0];
            usedControlScheme = controlSchemeNameKeyboard;
            if (myDevice is Gamepad)
                usedControlScheme = controlSchemeNameGamepad;

            InputIconsManagerSO.localMultiplayerManagement.AssignDeviceToPlayer(playerID, input.devices[0], false);
            SetControlSchemeName(usedControlScheme);
        }
    }

    public void SetControlSchemeName(string controlSchemeName)
    {
        usedControlScheme = controlSchemeName;
        InputIconsManagerSO.localMultiplayerManagement.SetControlSchemeForPlayer(playerID, controlSchemeName);

        if (playerPrompts)
            playerPrompts.SetActive(true);

        InputIconsManagerSO.onInputUsersChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if(useAutomaticID)
            nextID--;
    }

   
}
