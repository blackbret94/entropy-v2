using Entropy.Scripts.Player;
using UnityEngine;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionInfoPaneMultipanel : MultiPanel
    {

        public void SetClass(ClassDefinition definition)
        {
            ((ClassSelectionInfoPane)GetPanel()).SetClass(definition);
        }
    }
}