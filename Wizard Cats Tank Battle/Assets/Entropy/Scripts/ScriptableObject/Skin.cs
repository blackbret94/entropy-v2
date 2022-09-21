using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Entropy/Skin", order = 1)]
    public class Skin : ScriptableWardrobeItem
    {
        public int SkinId;
        public string SkinName;
        public Material SkinMaterial;
    }
}