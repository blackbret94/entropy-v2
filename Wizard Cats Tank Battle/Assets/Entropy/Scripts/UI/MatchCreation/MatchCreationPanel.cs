using Photon.Pun;
using Photon.Realtime;
using TanksMP;
using TMPro;
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

        public Text MapTitleText;
        public Text GameModeText;
        public TextMeshProUGUI HeaderText;

        public GameObject MultiplayerOnlyFieldsRoot;
        public GameObject SingleplayerOnlyFieldsRoot;

        private bool _isMultiplayer;

        private void Start()
        {
            Init();
        }

        public void ToggleMultiplayer(bool isMultiplayer)
        {
            _isMultiplayer = isMultiplayer;
            
            MultiplayerOnlyFieldsRoot.SetActive(_isMultiplayer);
            SingleplayerOnlyFieldsRoot.SetActive(!_isMultiplayer);

            if (_isMultiplayer)
            {
                // Connect to server
                PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);

                if (!PhotonNetwork.IsConnected)
                {
                    NetworkManagerCustom.GetInstance().Connect(NetworkMode.Online);
                }

                HeaderText.text = "Create Match";
            }
            else
            {
                // Disconnect from server
                PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Offline);
            
                if (PhotonNetwork.IsConnected)
                {
                    NetworkManagerCustom.GetInstance().DisconnectFromServer();
                }

                HeaderText.text = "Practice Against Bots";
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
            
            SetMapTitleText();
            SetGameModeTitleText();
        }
        
        public void CreateMatch()
        {
            if (_isMultiplayer)
            {
                CreateMatchMultiplayer();
            }
            else
            {
                CreateMatchSingleplayer();
            }
        }

        private void CreateMatchMultiplayer()
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
            UIMain.GetInstance().RoomController.CreateRoom(roomOptions);
        }

        private void CreateMatchSingleplayer()
        {
            string mapName = GetMapName();
            TanksMP.GameMode gameMode = GetGameMode();
            
            UIMain.GetInstance().RoomController.PlayOffline(mapName, (int)gameMode);
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

        public void SetMapTitleText()
        {
            MapTitleText.text = "Map: " + (MapSelector.IsRandom() ? "Random" : GetMapName());
        }

        public void SetGameModeTitleText()
        {
            GameModeText.text = "Game Mode: " + (GameModeSelector.IsRandom() ? "Random" : GameModeSelector.SelectedGameMode().Title);
        }
    }
}