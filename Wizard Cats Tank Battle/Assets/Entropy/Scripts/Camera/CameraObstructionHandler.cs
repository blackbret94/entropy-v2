using System.Collections.Generic;
using Entropy.Scripts.Player;
using UnityEngine;

namespace Vashta.Entropy.CameraControls
{
    public class CameraObstructionHandler : MonoBehaviour
    {
        [Header("References")]
        public Transform player;

        [Header("Settings")]
        [Tooltip("Select all layers that can obstruct the camera.")]
        public LayerMask obstructionLayers;

        [Tooltip("Speed of fade in/out transitions.")]
        public float fadeSpeed = 5f;

        [Tooltip("Alpha value for faded (transparent) objects.")]
        [Range(0f, 1f)] public float fadedAlpha = 0.3f;

        // Internal storage
        private readonly Dictionary<Renderer, Material[]> originalMaterials = new();
        private readonly HashSet<Renderer> currentObstructions = new();
        
        private void Update()
        {
            if(!player)
                GetLocalPlayer();
            
            if(player)
                HandleObstructions();
        }

        private void GetLocalPlayer()
        {
            if(PlayerList.GetLocalPlayer())
                player = PlayerList.GetLocalPlayer().transform;
        }

        private void HandleObstructions()
        {
            // Restore transparency to any renderers no longer obstructing
            List<Renderer> toRestore = new();
            foreach (var rend in currentObstructions)
            {
                if (rend == null)
                {
                    toRestore.Add(rend);
                    continue;
                }

                foreach (Material mat in rend.materials)
                {
                    if (!mat.HasProperty("_Color"))
                        continue;

                    Color c = mat.color;
                    c.a = Mathf.MoveTowards(c.a, 1f, fadeSpeed * Time.deltaTime);
                    mat.color = c;

                    if (Mathf.Approximately(c.a, 1f))
                        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }

                // If all materials are back to full opacity, mark for removal
                if (AllMaterialsAtFullOpacity(rend))
                    toRestore.Add(rend);
            }

            // Remove fully visible ones
            foreach (var rend in toRestore)
                currentObstructions.Remove(rend);

            // Detect new obstructions
            Vector3 dir = player.position - transform.position;
            float dist = Vector3.Distance(player.position, transform.position);
            RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, dist, obstructionLayers);

            foreach (RaycastHit hit in hits)
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend == null)
                    continue;

                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.materials;

                // Fade out each material
                foreach (Material mat in rend.materials)
                {
                    if (!mat.HasProperty("_Color"))
                        continue;

                    Color c = mat.color;
                    c.a = Mathf.MoveTowards(c.a, fadedAlpha, fadeSpeed * Time.deltaTime);
                    mat.color = c;
                }

                // rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                currentObstructions.Add(rend);
            }
        }
        
        private bool AllMaterialsAtFullOpacity(Renderer rend)
        {
            foreach (Material mat in rend.materials)
            {
                if (!mat.HasProperty("_Color"))
                    continue;
                if (mat.color.a < 0.99f)
                    return false;
            }
            return true;
        }
    }
}