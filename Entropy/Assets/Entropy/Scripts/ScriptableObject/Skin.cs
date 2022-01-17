using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Entropy/Skin", order = 1)]
    public class Skin : UnityEngine.ScriptableObject
    {
        public int SkinId;
        public string SkinName;
        public Material SkinMaterial;
    }
}