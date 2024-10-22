using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Team Definition", menuName = "Entropy/Team", order = 1)]
    public class TeamDefinition : UnityEngine.ScriptableObject
    {
        public string TeamId;
        public string TeamNameDisplay;
        public string TeamDescription;
        public Color TeamColorPrim;
        public Color TeamColorSec;
        public Color TeamColorTert;
        public Sprite HealthBarSprite;
        public Sprite ShieldBarSprite;
        public Material Material;
        public Material BarrierMaterial;
        public Material AnimatedBarrierMaterial;
        public Material FlagMaterial;
        public Color TeamLightColorPrim;
        public Color TeamLightColorSec;

        public Color GetPrimaryColorLight()
        {
            float blendFactor = .5f;
            return Color.Lerp(TeamColorPrim, Color.white, blendFactor);
        }
    }
}