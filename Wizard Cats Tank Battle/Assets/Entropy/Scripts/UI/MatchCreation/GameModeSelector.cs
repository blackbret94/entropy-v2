using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class GameModeSelector : MonoBehaviour
    {
        public MatchCreationPanel MatchCreationPanel;
        public MapSelectionSelector MapSelectionSelector;
        public GameModeDictionary GameModeDictionary;
        public List<GameModeCheckbox> GameModeCheckboxes;
        private GameModeCheckbox _activeSelection;

        private void Start()
        {
            if (GameModeCheckboxes.Count > 0)
                _activeSelection = GameModeCheckboxes[0];
        }

        public void SelectGameMode(GameModeCheckbox checkbox)
        {
            ResetCheckboxes();
            checkbox.Toggle(true);
            _activeSelection = checkbox;
            MapDefinition mapDefinition = MapSelectionSelector.SelectedMapDefinition();

            int idToSave = checkbox.GameModeDefinition ? 
                // Use selected game mode
                (int)checkbox.GameModeDefinition.GameMode :
                // Choose random game mode supported by map, or default TDM
                (int)(mapDefinition ? mapDefinition.GetRandomGamemode() : TanksMP.GameMode.TDM);
            
            PlayerPrefs.SetInt(PrefsKeys.gameMode, idToSave);
            MatchCreationPanel.SetGameModeTitleText();
        }

        // Null implies random
        public void SetCheckboxesToMap(MapDefinition mapDefinition)
        {
            foreach (GameModeCheckbox checkbox in GameModeCheckboxes)
            {
                checkbox.SetToMap(mapDefinition);
            }
        }

        public void SetToRandom()
        {
            ResetCheckboxes();

            if (GameModeCheckboxes.Count > 0)
            {
                _activeSelection = GameModeCheckboxes[0];
                GameModeCheckboxes[0].Toggle(true);
            }
            
            MatchCreationPanel.SetGameModeTitleText();
        }
        
        private void ResetCheckboxes()
        {
            foreach (var checkbox in GameModeCheckboxes)
            {
                checkbox.Toggle(false);
            }
        }

        public GameModeDefinition SelectedGameMode()
        {
            if (_activeSelection == null || _activeSelection.IsRandom)
                return GameModeDictionary.GetRandom();

            return _activeSelection.GameModeDefinition;
        }

        public bool IsRandom()
        {
            return !_activeSelection || _activeSelection.IsRandom;
        }
    }
}