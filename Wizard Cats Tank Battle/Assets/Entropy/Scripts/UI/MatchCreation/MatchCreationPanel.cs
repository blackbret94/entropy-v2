using Fusion;
using Newtonsoft.Json;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.Network;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.Scripts.CBSIntegration;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class MatchCreationPanel : GamePanel
    {
        public InputField NameInputField;
        public InputField PasswordInputField;
        public InputField MaxPlayersInputField;
        public Toggle IsPrivateToggle;
        public MapSelectionSelector MapSelector;
        public GameModeSelector GameModeSelector;

        public RoomOptionsFactory RoomOptionsFactory;

        public Text MapTitleText;
        public Text GameModeText;
        public TextMeshProUGUI HeaderText;

        public GameObject MultiplayerOnlyFieldsRoot;
        public GameObject SingleplayerOnlyFieldsRoot;
        public GameObject CreateMatchOnlyFieldsRoot;
        
        private bool _isMultiplayer;
        private NetworkManagerCustom _networkManagerCustom;
        private MatchPanelType _matchPanelType;

        private void Start()
        {
            Init();
        }

        public void ToggleMultiplayer(MatchPanelType type)
        {
            _matchPanelType = type;

            // This is done in an order to force the UI to refresh the layout
            // Refresh the sub layout first
            CreateMatchOnlyFieldsRoot.SetActive(type == MatchPanelType.Create);
            
            // Force the top level root to refresh
            MultiplayerOnlyFieldsRoot.SetActive(false);
            SingleplayerOnlyFieldsRoot.SetActive(false);

            // Re-enable the correct panel
            _isMultiplayer = (type is MatchPanelType.Create or MatchPanelType.Matchmaking);
            if (_isMultiplayer)
            {
                MultiplayerOnlyFieldsRoot.SetActive(true);
            }
            else
            {
                SingleplayerOnlyFieldsRoot.SetActive(true);
            }

            switch (type)
            {
                case MatchPanelType.Practice:
                    PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Offline);
                    HeaderText.text = "Practice Against Bots";
                    break;
                
                case MatchPanelType.Matchmaking:
                    PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);
                    HeaderText.text = "Matchmaking";
                    break;
                
                case MatchPanelType.Create:
                    PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);
                    HeaderText.text = "Create";
                    break;
            }
        }

        private void Init()
        {
            _networkManagerCustom = NetworkManagerCustom.GetInstance();
            
            if (NameInputField == null)
            {
                Debug.LogError("Match creation panel is missing a name input field!");
            }
            else
            {
                NameInputField.text = RoomOptionsFactory.CreateRoomNameFromPlayerNickname(CBSIntegrator.Instance.ProfileState.CachedDisplayName);
            }
            
            SetMapTitleText();
            SetGameModeTitleText();
        }
        
        public void CreateMatch()
        {
            // Get info
            string roomName = GetRoomName();
            bool isPrivate = IsPrivateToggle.isOn;
            
            // format
            roomName = RoomOptionsFactory.CreateRoomName(roomName);
            
            MatchmakingArgs matchmakingArgs = new MatchmakingArgs(_isMultiplayer, roomName, GetPassword(), GetMaxPlayers(), 
                GetMapName(), GetGameMode(), !isPrivate);

            string encrypted = matchmakingArgs.Encrypt();
            PlayerPrefs.SetString(SaveLoad.PrefsKeys.matchmakingArgs, encrypted);
            
            StartGameArgs startGameArgs = RoomOptionsFactory.CreateRoomOptions(matchmakingArgs);
            
            // create room
            UIMain.GetInstance().roomConnectionController.CreateRoom(startGameArgs);
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
            if (MapSelector.IsRandom())
                return "random";
            
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