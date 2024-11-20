using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Minimap
{
    public class MinimapEntityControllerHomeBase : MinimapEntityController
    {
        public TeamDefinition TeamDefinition;
        
        protected override void PostInit()
        {
            SetEntityColor(TeamDefinition.TeamColorPrim);
        }
    }
}