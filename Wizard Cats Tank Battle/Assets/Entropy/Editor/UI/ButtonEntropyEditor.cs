using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;
using Vashta.Entropy.UI;

namespace Vashta.Entropy.Editor.UI
{
    [CustomEditor(typeof(ButtonEntropy))]
    public class ButtonEntropyEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            ButtonEntropy targetButtonEntropy = (ButtonEntropy)target;
            targetButtonEntropy.rootImage =
                (Image)EditorGUILayout.ObjectField("Root Image:", targetButtonEntropy.rootImage, typeof(Image), true);
        }
    }
}
