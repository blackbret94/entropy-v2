using System.Collections.Generic;
using TanksMP;
using UnityEngine;

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
                _activeSelection = ClassSelectionTeamCheckboxes[0];
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
            if (_activeSelection.IsAutoAssign)
                return PlayerExtensions.RANDOM_TEAM_INDEX;

            return _activeSelection.TeamIndex;
        }

    }
}