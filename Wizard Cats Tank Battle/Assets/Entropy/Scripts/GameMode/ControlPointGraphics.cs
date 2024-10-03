using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class ControlPointGraphics : MonoBehaviour
    {
        public MeshRenderer GroundMesh;
        public List<TeamColorChangingProp> Props;

        public void ChangeTeamColor(TeamDefinition team)
        {
            if (!team)
            {
                Debug.LogError("Attempted to change a control point colors to a null team!");
            }
            
            GroundMesh.material = team.Material;
            foreach (TeamColorChangingProp prop in Props)
            {
                prop.SetTeamColors(team);
            }
        }
    }
}