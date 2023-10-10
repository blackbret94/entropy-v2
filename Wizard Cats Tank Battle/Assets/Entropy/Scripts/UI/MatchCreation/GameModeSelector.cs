using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class GameModeSelector : MonoBehaviour
    {
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

            string idToSave = checkbox.GameModeDefinition ? checkbox.GameModeDefinition.Id : "-1";
            PlayerPrefs.SetString(PrefsKeys.gameMode, idToSave);
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
            if (_activeSelection == null)
                return null;

            return _activeSelection.GameModeDefinition;
        }
    }
}