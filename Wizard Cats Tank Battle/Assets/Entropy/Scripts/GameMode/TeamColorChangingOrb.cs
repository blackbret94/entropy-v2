using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class TeamColorChangingOrb : TeamColorChangingProp
    {
        public Light Light;
        public MeshRenderer OrbMesh;
        
        public override void SetTeamColors(TeamDefinition teamDefinition)
        {
            if(teamDefinition)
            {
                // Change material on flag
                if (OrbMesh)
                {
                    OrbMesh.material = teamDefinition.Material;
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