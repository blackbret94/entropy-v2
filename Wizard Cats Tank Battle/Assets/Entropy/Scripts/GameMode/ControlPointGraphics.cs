using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class ControlPointGraphics : MonoBehaviour
    {
        public List<MeshRenderer> MeshesForControl;
        public List<TeamColorChangingProp> PropsForControl; // These show who CONTROLS the point
        public List<TeamColorChangingProp> PropsForCapturing; // These show the CAPTURE STATUS of the point
        public ElevationChangingProp FlagRoot;
        public SpriteRenderer ProgressRadialRenderer;
        private Material _progressRadialMaterial;
        private static readonly int ArcValue = Shader.PropertyToID("_ArcValue");
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        private void Start()
        {
            FlagRoot.SetElevation(0);
            _progressRadialMaterial = ProgressRadialRenderer.material;
            _progressRadialMaterial.SetFloat(ArcValue, 0);
        }

        // Has the CONTROL changed?
        public void ChangeTeamColorControl(TeamDefinition team)
        {
            if (!team)
            {
                Debug.LogError("Attempted to change a control point colors to a null team!");
            }

            foreach (MeshRenderer mesh in MeshesForControl)
            {
                mesh.material = team.AnimatedBarrierMaterial;
            }
            
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
            
            _progressRadialMaterial.SetColor(Color1, team.TeamColorPrim);
        }

        public void SetFlagPosition(float percentage)
        {
            FlagRoot.SetElevation(percentage);
            _progressRadialMaterial.SetFloat(ArcValue, percentage);
        }
    }
}