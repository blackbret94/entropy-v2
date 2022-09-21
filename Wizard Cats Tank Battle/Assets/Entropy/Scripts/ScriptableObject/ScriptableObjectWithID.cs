// Adapted From https://stackoverflow.com/questions/58984486/create-scriptable-object-with-constant-unique-id
using System;
using UnityEditor;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
    public class ScriptableObjectIdDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            if (string.IsNullOrEmpty(property.stringValue)) {
                property.stringValue = Guid.NewGuid().ToString();
            }
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
    
    public class ScriptableObjectWithID : UnityEngine.ScriptableObject
    {
        [ScriptableObjectId]
        public string Id;
    }
}