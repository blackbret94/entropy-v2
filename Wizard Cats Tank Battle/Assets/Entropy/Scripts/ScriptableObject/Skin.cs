using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Entropy/Skin", order = 1)]
    public class Skin : ScriptableWardrobeItem
    {
        public string SkinName;
        public Material SkinMaterial;
        public override WardrobeCategory Category => WardrobeCategory.SKIN;
    }
}