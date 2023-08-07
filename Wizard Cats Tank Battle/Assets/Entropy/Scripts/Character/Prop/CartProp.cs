
using UnityEngine;
using UnityEngine.Serialization;

namespace Vashta.Entropy.Character.Prop
{
    public class CartProp: MonoBehaviour
    {
        [Tooltip("Changes materials in the first slot")]
        [FormerlySerializedAs("TeamColorMeshes")] 
        public MeshRenderer[] TeamColorMeshesPrimary;
        [Tooltip("Changes materials in the second slot")]
        public MeshRenderer[] TeamColorMeshesSecondary;

        public void ColorizeForTeam(Material material)
        {
            for (int i = 0; i < TeamColorMeshesPrimary.Length; i++)
                TeamColorMeshesPrimary[i].material = material;
            
            for (int i = 0; i < TeamColorMeshesSecondary.Length; i++)
                TeamColorMeshesSecondary[i].materials[1] = material;
        }
    }
}