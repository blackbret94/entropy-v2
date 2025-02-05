using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.GameState;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionTeamSelector : MonoBehaviour
    {
        public List<ClassSelectionTeamCheckbox> ClassSelectionTeamCheckboxes;

        private ClassSelectionTeamCheckbox _activeSelection;

        private void Start()
        {
            if (ClassSelectionTeamCheckboxes.Count > 0)
            {
                _activeSelection = null;
            }
        }

        public void SelectTeam(ClassSelectionTeamCheckbox checkbox)
        {
            ResetCheckboxes();
            checkbox.Toggle(true);
            _activeSelection = checkbox;
        }
        
        private void ResetCheckboxes()
        {
            foreach (var checkbox in ClassSelectionTeamCheckboxes)
            {
                checkbox.Toggle(false);
            }
        }

        public int SelectedTeamIndex()
        {
            // Handle no selection was made
            if (!_activeSelection)
                return -1;
            
            if (_activeSelection.IsAutoAssign)
                return TeamController.RANDOM_TEAM_INDEX;

            return _activeSelection.TeamIndex;
        }

    }
}