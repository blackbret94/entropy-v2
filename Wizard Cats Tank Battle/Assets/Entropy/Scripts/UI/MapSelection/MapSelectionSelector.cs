using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.UI.MatchCreation;

namespace Vashta.Entropy.UI.MapSelection
{
    public class MapSelectionSelector : MonoBehaviour
    {
        public MapDefinitionDictionary MapDefinitionDictionary;
        public List<MapSelectionCheckbox> MapSelectionCheckboxes;
        public GameModeSelector GameModeSelector;

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

            string mapIdToSave = checkbox.mapDefinition ? checkbox.mapDefinition.Id : "-1";
            PlayerPrefs.SetString(PrefsKeys.selectedMap, mapIdToSave);
            
            GameModeSelector.SetCheckboxesToMap(checkbox.mapDefinition);
            GameModeSelector.SetToRandom();
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
            if (_activeSelection == null || _activeSelection.IsRandom)
                return MapDefinitionDictionary.GetRandom();

            return _activeSelection.mapDefinition;
        }

    }
}