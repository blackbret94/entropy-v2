using System.Collections.Generic;
using Entropy.Scripts.Player;
using UnityEngine;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionSelector : MonoBehaviour
    {
        public List<ClassSelectionCheckbox> ClassSelectionCheckboxes;
        public ClassSelectionInfoPane InfoPane;
        
        private ClassSelectionCheckbox _activeSelection;

        private void Start()
        {
            if (ClassSelectionCheckboxes.Count > 0)
                _activeSelection = ClassSelectionCheckboxes[0];
            
            InfoPane.SetClass(_activeSelection.ClassDefinition);
        }
        
        public void SelectClass(ClassSelectionCheckbox checkbox)
        {
            ResetCheckboxes();
            checkbox.Toggle(true);
            _activeSelection = checkbox;
            InfoPane.SetClass(checkbox.ClassDefinition);
        }

        private void ResetCheckboxes()
        {
            foreach (var checkbox in ClassSelectionCheckboxes)
            {
                checkbox.Toggle(false);
            }
        }

        public ClassDefinition SelectedClassDefinition()
        {
            if (_activeSelection == null)
            {
                return null;
            }

            return _activeSelection.ClassDefinition;
        }
    }
}