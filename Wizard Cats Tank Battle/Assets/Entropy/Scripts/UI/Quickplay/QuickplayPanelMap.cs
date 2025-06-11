using System.Collections.Generic;
using Fusion;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.Quickplay
{
    public class QuickplayPanelMap : GamePanel
    {
        public QuickplayPanel QuickplayPanel;
        public GameObject MapButtonPrefab;
        public Transform MapButtonsRoot;
        public MapDefinitionDictionary MapDefinitionDictionary;
        public TextMeshProUGUI GameModeTitleText;
        public TextMeshProUGUI GameModeDescriptionText;
        public Image GameModeImage;
        public RoomOptionsFactory RoomOptionsFactory;

        private GameModeDefinition _gameModeDefinition;
        
        public void Inflate(GameModeDefinition gameModeDefinition)
        {
            _gameModeDefinition = gameModeDefinition;
            
            // Clean up
            foreach (Transform obj in MapButtonsRoot.transform)
            {
                Destroy(obj.gameObject);
            }
            
            // Set text
            GameModeTitleText.text = _gameModeDefinition.Title;
            GameModeTitleText.color = _gameModeDefinition.Color;
            
            if(GameModeDescriptionText)
                GameModeDescriptionText.text = _gameModeDefinition.Description;
            
            if(GameModeImage)
                GameModeImage.sprite = _gameModeDefinition.Icon;
            
            // Get maps supported by game mode
            List<MapDefinition> mapDefinitions = MapDefinitionDictionary.GetMapsSupportingGameMode(gameModeDefinition.GameMode);

            // Instantiate
            foreach (MapDefinition mapDefinition in mapDefinitions)
            {
                GameObject mapButton = Instantiate(MapButtonPrefab, MapButtonsRoot);
                
                // Inflate button
                QuickplayMapButton button = mapButton.GetComponent<QuickplayMapButton>();
                button.Inflate(mapDefinition, this);
            }
        }
        
        public void StartRandom()
        {
            // Random map
            StartGameArgs startGameArgs = RoomOptionsFactory.QuickplayArgs();
            UIMain.GetInstance().roomConnectionController.CreateRoom(startGameArgs);
        }

        public void Back()
        {
            // Go back to gameplay panel mode
            QuickplayPanel.OpenMode();
        }

        public void StartMap(MapDefinition mapDefinition)
        {
            StartGameArgs startGameArgs = RoomOptionsFactory.QuickplayArgs();
            UIMain.GetInstance().roomConnectionController.CreateRoom(startGameArgs);
        }
    }
}