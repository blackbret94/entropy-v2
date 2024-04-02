using UnityEngine;

///#IGNORE
#if UNITY_EDITOR
using UnityEditor;

using VrGamesDev.BHEL;

///#IGNORE
[CustomEditor(typeof(VRG_Bhel))]
class VRG_BhelEditor : Editor
{
    ///#IGNORE
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Open Html in a Browser"))
        {
            if (Application.isPlaying)
            {
                if (VRG_Bhel.Instance != null)
                {
                    VRG_Bhel.Instance.OpenUrl();
                }
                else
                {
                    UnityEngine.Debug.Log("Play the game and INIT the VRG_Bhel to open the BHEL browser");
                }
            }
            else
            {
                UnityEngine.Debug.Log("Play the game to open the BHEL browser");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}
#endif