using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class PracticePanel : GamePanel
    {
        public MapSelectionSelector MapSelector;
        public GameModeSelector GameModeSelector;
        
        public override void OpenPanel()
        {
            base.OpenPanel();

            if (PhotonNetwork.IsConnected)
            {
                PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Offline);
                NetworkManagerCustom.GetInstance().DisconnectFromServer();
            }
        }
        
        public void CreateMatch()
        {
            // Get info
            string mapName = GetMapName();
            TanksMP.GameMode gameMode = GetGameMode();
            
            UIMain.GetInstance().PlayOffline(mapName, (int)gameMode);
        }

        private TanksMP.GameMode GetGameMode()
        {
            if (!GameModeSelector)
            {
                Debug.LogError("Missing connection to GameMode Selector!");
            }
            
            GameModeDefinition definition = GameModeSelector.SelectedGameMode();

            if (!definition)
            {
                Debug.LogError("Could not find definition in gamemode selector!");
            }
            
            return definition.GameMode;
        }
        
        private string GetMapName()
        {
            MapDefinition mapDefinition = MapSelector.SelectedMapDefinition();
            
            if(!mapDefinition)
            {
                Debug.LogError("Missing a selected map!");
                return "";
            }
            
            return mapDefinition.Title;
        }
    }
}