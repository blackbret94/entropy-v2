using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Entropy/Skin", order = 1)]
    public class Skin : ScriptableWardrobeItem
    {
        public string SkinName;
        public Material SkinMaterial;
    }
}