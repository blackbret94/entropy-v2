using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "BodyType", menuName = "Entropy/BodyType", order = 1)]
    public class BodyType: ScriptableWardrobeItem
    {
        public string BodyTypeName;
        public Mesh BodyMesh;
        public List<Skin> SkinOptions;
        public override WardrobeCategory Category => WardrobeCategory.BODY_TYPE;

        public Skin GetRandomSkin()
        {
            return SkinOptions[Random.Range(0, SkinOptions.Count)];
        }
    }
}