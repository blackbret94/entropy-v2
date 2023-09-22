#if ENABLE_INPUT_SYSTEM && UNITY_EDITOR
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using System;
using System.Linq;

namespace Cooldhands
{
    // The only purpose of the Input System suppying a custom editor for the UI StandaloneInputModule is to guide users to using
    // the Input System's InputSystemUIInputModule instead.
    [CustomEditor(typeof(UICursorStandaloneInputModule))]
    internal class UICursorStandaloneInputModuleEditor : UnityEditor.Editor
    {
        SerializedProperty enableNativePlatformBackendsForNewInputSystem;
        SerializedProperty disableOldInputManagerSupport;

        public void OnEnable()
        {
            var allPlayerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
            if (allPlayerSettings.Length > 0)
            {
                var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>()[0];
                var so = new SerializedObject(playerSettings);
                enableNativePlatformBackendsForNewInputSystem = so.FindProperty("enableNativePlatformBackendsForNewInputSystem");
                disableOldInputManagerSupport = so.FindProperty("disableOldInputManagerSupport");
            }
        }

        private static InputActionReference GetActionReferenceFromAssets(InputActionReference[] actions, params string[] actionNames)
        {
            foreach (var actionName in actionNames)
            {
                foreach (var action in actions)
                {
                    if (string.Compare(action.action.name, actionName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        return action;
                }
            }
            return null;
        }

        private static InputActionReference[] GetAllActionsFromAsset(InputActionAsset actions)
        {
            if (actions != null)
            {
                var path = AssetDatabase.GetAssetPath(actions);
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                return assets.Where(asset => asset is InputActionReference).Cast<InputActionReference>().OrderBy(x => x.name).ToArray();
            }
            return null;
        }

        public override void OnInspectorGUI()
        {
            // We assume that if these properties don't exist (ie are null), then that's because the new Input System has become the default.
            if (enableNativePlatformBackendsForNewInputSystem == null || enableNativePlatformBackendsForNewInputSystem.boolValue)
            {
                if (disableOldInputManagerSupport == null || disableOldInputManagerSupport.boolValue)
                    EditorGUILayout.HelpBox("You are using UICursorStandaloneInputModule, which uses the old InputManager. You are using the new InputSystem, and have the old InputManager disabled. StandaloneInputModule will not work. Click the button below to replace this component with a InputSystemUIInputModule, which uses the new InputSystem.", MessageType.Error);
                else
                    EditorGUILayout.HelpBox("You are using UICursorStandaloneInputModule, which uses the old InputManager. You also have the new InputSystem enabled in your project. Click the button below to replace this component with a InputSystemUIInputModule, which uses the new InputSystem (recommended).", MessageType.Info);
                if (GUILayout.Button("Replace with InputSystemUIInputModule"))
                {
                    UICursorStandaloneInputModule t = (UICursorStandaloneInputModule)target;
                    var go = (t).gameObject;
                    Undo.DestroyObjectImmediate(target);
                    Undo.AddComponent<InputSystemUIInputModule>(go);
                    Undo.AddComponent<UICursorOverRaycaster>(go);
                    UICursorVirtualMouseInput mouseVirtual = Undo.AddComponent<UICursorVirtualMouseInput>(go);
                    mouseVirtual.Cursor = t.m_cursor;

                    string[] guids =  AssetDatabase.FindAssets("UICursorActions t:InputActionAsset");
                    if(guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        InputActionAsset action = AssetDatabase.LoadAssetAtPath(path, typeof(InputActionAsset)) as InputActionAsset;
                        var assets = GetAllActionsFromAsset(action);
                        mouseVirtual.stickAction = new InputActionProperty(GetActionReferenceFromAssets(assets, "Point"));
                        mouseVirtual.clickAction = new InputActionProperty(GetActionReferenceFromAssets(assets, "Click"));
                        mouseVirtual.scrollWheelAction = new InputActionProperty(GetActionReferenceFromAssets(assets, "Scroll"));
                    }
                    return;
                }
                 
                GUILayout.Space(10);
            }
            base.OnInspectorGUI();
        }
    }
}
#endif