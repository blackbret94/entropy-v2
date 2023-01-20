using UnityEngine;

namespace Vashta.Entropy.Character.Prop
{
    public class CartProp: MonoBehaviour
    {
        public MeshRenderer[] TeamColorMeshes;
        
        public void ColorizeForTeam(Material material)
        {
            Debug.Log("Colorizing cart with materials: " + material.color);
            
            for (int i = 0; i < TeamColorMeshes.Length; i++)
                TeamColorMeshes[i].material = material;
        }
    }
}