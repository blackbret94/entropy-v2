using UnityEngine;

namespace Vashta.Entropy.Character.Prop
{
    public class CartProp: MonoBehaviour
    {
        public MeshRenderer[] TeamColorMeshes;
        
        public void ColorizeForTeam(Material material)
        {
            for (int i = 0; i < TeamColorMeshes.Length; i++)
                TeamColorMeshes[i].material = material;
        }
    }
}