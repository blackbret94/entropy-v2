using InputIcons;
using UnityEngine;

public class InputIconsManagerInitializer
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitialized()
    {
        InputIconsManagerSO.Instance.Initialize();
    }
}
