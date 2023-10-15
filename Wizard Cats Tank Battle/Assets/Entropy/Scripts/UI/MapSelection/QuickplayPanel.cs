using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.UI.MatchCreation;

namespace Vashta.Entropy.UI.MapSelection
{
    public class QuickplayPanel : GamePanel
    {
        public MapSelectionSelector MapSelectionSelector;
        public GameModeSelector GameModeSelector;

        private const string DefaultMapName = "";
        private const int DefaultGameMode = (int)TanksMP.GameMode.TDM;

        public override void OpenPanel()
        {
            base.OpenPanel();

            if (!PhotonNetwork.IsConnected)
            {
                PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);
                NetworkManagerCustom.GetInstance().Connect(NetworkMode.Online);
            }
        }

        public void Play()
        {
            string mapName = GetMapName();
            int gameMode = GetGameMode();
            
            UIMain.GetInstance().Play(mapName, gameMode);
        }

        private string GetMapName()
        {
            if (!MapSelectionSelector)
            {
                Debug.LogError("Missing connection to map selection selector!");
                return DefaultMapName;
            }

            if (!MapSelectionSelector.SelectedMapDefinition())
            {
                Debug.LogError("Missing map definition!");
                return DefaultMapName;
            }

            return MapSelectionSelector.SelectedMapDefinition().Title;
        }

        private int GetGameMode()
        {
            if (!GameModeSelector)
            {
                Debug.LogError("Missing game mode selector!");
                return DefaultGameMode;
            }

            if (GameModeSelector.SelectedGameMode() == null)
            {
                Debug.LogError("Missing game mode selection");
                return DefaultGameMode;
            }

            return (int)GameModeSelector.SelectedGameMode().GameMode;
        }
    }
}