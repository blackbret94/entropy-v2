using System.Collections.Generic;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionPanel : GamePanel
    {
        public List<ClassSelectionTeamCheckbox> checkboxes;
        public int ActiveSelection = ClassSelectionTeamCheckbox.AUTO_ASSIGN_INDEX;
    }
}