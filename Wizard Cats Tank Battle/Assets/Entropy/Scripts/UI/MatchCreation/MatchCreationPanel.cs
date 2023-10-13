using Photon.Pun;
using Photon.Realtime;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class MatchCreationPanel : GamePanel
    {
        public InputField NameInputField;
        public InputField PasswordInputField;
        public InputField MaxPlayersInputField;
        public MapSelectionSelector MapSelector;
        public GameModeSelector GameModeSelector;

        public RoomOptionsFactory RoomOptionsFactory;

        private void Start()
        {
            Init();
        }

        public override void OpenPanel()
        {
            base.OpenPanel();

            if (!PhotonNetwork.IsConnected)
            {
                PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);
                NetworkManagerCustom.GetInstance().Connect(NetworkMode.Online);
            }
        }

        private void Init()
        {
            if (NameInputField == null)
            {
                Debug.LogError("Match creation panel is missing a name input field!");
            }
            else
            {
                NameInputField.text = RoomOptionsFactory.CreateRoomNameFromPlayerNickname(PhotonNetwork.NickName);
            }
        }
        
        public void CreateMatch()
        {
            // Get info
            string roomName = GetRoomName();
            // string password = GetPassword();
            int maxPlayers = GetMaxPlayers();
            string mapName = GetMapName();
            TanksMP.GameMode gameMode = GetGameMode();
            
            // format
            roomName = RoomOptionsFactory.CreateRoomName(roomName);
            RoomOptions roomOptions = RoomOptionsFactory.InitRoomOptions(roomName, mapName, maxPlayers, gameMode);
            
            // create room
            UIMain.GetInstance().CreateRoom(roomOptions);
        }
        
        private string GetRoomName()
        {
            if (NameInputField == null)
            {
                Debug.LogError("Match creation panel is missing a name input field!");
                return "";
            }

            if (NameInputField.text == "")
                return "";

            return NameInputField.text;
        }

        private string GetPassword()
        {
            return PasswordInputField.text;
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

        private int GetMaxPlayers()
        {
            if (!MaxPlayersInputField)
            {
                Debug.LogError("Match creation panel is missing a max players input field!");
                return 12;
            }
            
            ClampInputFieldInt clampInputFieldInt = MaxPlayersInputField.gameObject.GetComponent<ClampInputFieldInt>();

            if (!clampInputFieldInt)
            {
                Debug.LogError("Match creation panel max players input field is missing a clamp component");
                return 12;
            }
            
            return clampInputFieldInt.GetClampedValue();
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