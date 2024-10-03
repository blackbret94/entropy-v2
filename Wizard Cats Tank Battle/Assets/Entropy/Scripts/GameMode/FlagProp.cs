using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class FlagProp : TeamColorChangingProp
    {
        public MeshRenderer FlagMesh;
        public Light Light;

        // TODO: Add support for auto-changing when the winning team changes.  this would register with gamemanager

        public override void SetTeamColors(TeamDefinition teamDefinition)
        {
            if(teamDefinition)
            {
                // Change material on flag
                if (FlagMesh)
                {
                    FlagMesh.material = teamDefinition.FlagMaterial;
                }

                // Change color on light
                if (Light)
                {
                    Light.color = teamDefinition.TeamLightColorPrim;
                }
            }
        }
    }
}