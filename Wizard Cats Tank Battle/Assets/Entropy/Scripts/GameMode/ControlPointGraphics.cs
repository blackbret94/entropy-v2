using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class ControlPointGraphics : MonoBehaviour
    {
        public MeshRenderer GroundMesh;
        public List<TeamColorChangingProp> PropsForControl; // These show who CONTROLS the point
        public List<TeamColorChangingProp> PropsForCapturing; // These show the CAPTURE STATUS of the point
        public ElevationChangingProp FlagRoot;

        private void Start()
        {
            FlagRoot.SetElevation(0);
        }

        // Has the CONTROL changed?
        public void ChangeTeamColorControl(TeamDefinition team)
        {
            if (!team)
            {
                Debug.LogError("Attempted to change a control point colors to a null team!");
            }
            
            GroundMesh.material = team.BarrierMaterial;
            foreach (TeamColorChangingProp prop in PropsForControl)
            {
                prop.SetTeamColors(team);
            }
            
            foreach (TeamColorChangingProp prop in PropsForCapturing)
            {
                prop.SetTeamColors(team);
            }
        }

        // Is someone in the process of CAPTURING it?
        public void ChangeTeamColorCapturing(TeamDefinition team)
        {
            foreach (TeamColorChangingProp prop in PropsForCapturing)
            {
                prop.SetTeamColors(team);
            }
        }

        public void SetFlagPosition(float percentage)
        {
            FlagRoot.SetElevation(percentage);
        }
    }
}