using System;
using TanksMP;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI.MapSelection
{
    public class MapSelectionPanel : GamePanel
    {
        public TextMeshProUGUI NetworkModeText;
        
        public MapDefinitionDictionary MapDefinitionDictionary;
        public MapSelectionSelector MapSelectionSelector;
        // Add game mode

        public override void OpenPanel()
        {
            base.OpenPanel();

            NetworkMode networkMode = (NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode, 0);

            switch (networkMode)
            {
                case NetworkMode.Online:
                    NetworkModeText.text = "Multiplayer";
                    break;
                case NetworkMode.Offline:
                    NetworkModeText.text = "Singleplayer";
                    break;
                default:
                    NetworkModeText.text = "";
                    break;
            }
        }
    }
}