using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "BodyType", menuName = "Entropy/BodyType", order = 1)]
    public class BodyType: ScriptableWardrobeItem
    {
        public string BodyTypeName;
        public Mesh BodyMesh;
        public List<Skin> SkinOptions;

        public Skin GetRandomSkin()
        {
            return SkinOptions[Random.Range(0, SkinOptions.Count)];
        }
    }
}