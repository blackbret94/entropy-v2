using UnityEditor;
using UnityEngine;

namespace Vashta.Entropy.Editor
{
    public class RandomYRotation : EditorWindow
    {
        [MenuItem("Tools/Randomize Y Rotation %r")] // Ctrl/Cmd + R
        static void RandomizeYRotation()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Undo.RecordObject(obj.transform, "Randomize Y Rotation");
                Vector3 rotation = obj.transform.eulerAngles;
                rotation.y = Random.Range(0f, 360f);
                obj.transform.eulerAngles = rotation;
            }
        }
    }
}