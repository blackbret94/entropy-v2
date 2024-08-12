using System.Collections.Generic;
using Entropy.Scripts.Player;
using UnityEngine;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionSelector : GamePanel
    {
        public List<ClassSelectionCheckbox> ClassSelectionCheckboxes;
        public ClassSelectionInfoPaneMultipanel InfoMultiPanel;
        
        private ClassSelectionCheckbox _activeSelection;

        private void Start()
        {
            if (ClassSelectionCheckboxes.Count > 0)
                _activeSelection = ClassSelectionCheckboxes[0];
            
            InitCheckboxes();
            
            // InfoPane.SetClass(_activeSelection.ClassDefinition);
            InfoMultiPanel.SetClass(_activeSelection.ClassDefinition);
        }

        private void InitCheckboxes()
        {
            for (int i = 0; i < ClassSelectionCheckboxes.Count; i++)
            {
                ClassSelectionCheckbox checkbox = ClassSelectionCheckboxes[i];
                if(checkbox)
                    checkbox.SetUI_Index(i);
            }
        }
        
        public void SelectClass(ClassSelectionCheckbox checkbox)
        {
            ResetCheckboxes();
            checkbox.Toggle(true);
            _activeSelection = checkbox;
            // InfoPane.SetClass(checkbox.ClassDefinition);
            InfoMultiPanel.SetClass(_activeSelection.ClassDefinition);
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

        public void SelectNext()
        {
            Debug.Log("Selecting next");
            if (_activeSelection != null)
            {
                int selectedId = _activeSelection.GetUI_Index(); // Should this be set in a loop at the start, via function?
                
                // cap
                int newId = selectedId + 1;

                if (newId >= ClassSelectionCheckboxes.Count)
                {
                    newId = 0;
                }
                
                Debug.Log("New ID: " + newId);
                
                // Set new selection
                ClassSelectionCheckbox checkbox = ClassSelectionCheckboxes[newId];

                if (checkbox)
                {
                    SelectClass(checkbox);
                }
            }
        }

        public void SelectPrevious()
        {
            Debug.Log("Selecting previous");
            
            if (_activeSelection != null)
            {
                int selectedId = _activeSelection.GetUI_Index(); // Should this be set in a loop at the start, via function?
                
                // cap
                int newId = selectedId - 1;

                if (newId < 0)
                {
                    newId = ClassSelectionCheckboxes.Count-1;
                }

                Debug.Log("New ID: " + newId);
                
                // Set new selection
                ClassSelectionCheckbox checkbox = ClassSelectionCheckboxes[newId];

                if (checkbox)
                {
                    SelectClass(checkbox);
                }
            }
        }
    }
}