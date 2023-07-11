using System.Collections.Generic;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI.MapSelection
{
    public class MapSelectionSelector : MonoBehaviour
    {
        public List<MapSelectionCheckbox> MapSelectionCheckboxes;

        private MapSelectionCheckbox _activeSelection;

        private void Start()
        {
            if (MapSelectionCheckboxes.Count > 0)
                _activeSelection = MapSelectionCheckboxes[0];
        }

        public void SelectMap(MapSelectionCheckbox checkbox)
        {
            ResetCheckboxes();
            checkbox.Toggle(true);
            _activeSelection = checkbox;
            PlayerPrefs.SetString(PrefsKeys.selectedMap, checkbox.mapDefinition.Id);
        }
        
        private void ResetCheckboxes()
        {
            foreach (var checkbox in MapSelectionCheckboxes)
            {
                checkbox.Toggle(false);
            }
        }

        public MapDefinition SelectedMapDefinition()
        {
            if (_activeSelection == null)
                return null;

            return _activeSelection.mapDefinition;
        }

    }
}