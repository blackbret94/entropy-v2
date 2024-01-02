using Entropy.Scripts.Player;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionSelectorMultipanel : MultiPanel
    {
        public ClassDefinition SelectedClassDefinition()
        {
            return ((ClassSelectionSelector)GetPanel()).SelectedClassDefinition();
        }
    }
}