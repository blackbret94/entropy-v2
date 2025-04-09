using Fusion;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.MatchBrowser
{
    public class MatchBrowserSelectorUnit : MonoBehaviour
    {
        [FormerlySerializedAs("Button")] public Button JoinButton;
        public Button CreateButton;
        public Image Image;
        public TextMeshProUGUI Text;
        public Image GameModeIcon;

        public GameModeDictionary GameModeDictionary;

        private string _roomName = "";
        private RoomInfoWrapper _roomInfoWrapper;
        
        public void InitUnit(SessionInfo room)
        {
            if (room == null) return;

            _roomInfoWrapper = new RoomInfoWrapper(room);
            
            // Format text
            _roomName = _roomInfoWrapper.GetDisplayRoomName();
            string roomString = _roomInfoWrapper.StringifyRoom();

            if (roomString == null) return;

            Text.text = roomString;
            
            // Format icon
            GameModeDefinition gameModeDefinition = GameModeDictionary[_roomInfoWrapper.GetGameMode()];
            GameModeIcon.sprite = gameModeDefinition.Icon;

            // Format widget
            if(room.IsOpen)
                ShowJoinButton();
            else
                HideButtons();
        }

        /// <summary>
        /// Display placeholder text if no rooms are found
        /// </summary>
        public void SetNoRoomsFound()
        {
            Text.text = "No matches could be found!  Create one instead!";
            ShowCreateButton();
            Image.enabled = false;
        }
        
        /// <summary>
        /// Wired to a button press to actually attempt to join the room
        /// </summary>
        public void JoinRoom()
        {
            string roomNameId = _roomInfoWrapper.GetRoomNameId();
            
            if (roomNameId == "")
            {
                Debug.LogError("Cannot join room without name!");
                return;
            }
            
            UIMain.GetInstance().roomConnectionController.CreateRoom(_roomInfoWrapper.GetStartGameArgs());
        }

        public void CreateRoom()
        {
            MatchmakingPanel matchmakingPanel = FindFirstObjectByType<MatchmakingPanel>();

            if (matchmakingPanel)
            {
                matchmakingPanel.ShowCreate();
            }
            else
            {
                Debug.LogError("Cannot find matchmaking panel!");
            }
        }
        
        private void ShowJoinButton()
        {
            HideButtons();
            JoinButton.gameObject.SetActive(true);
        }

        private void ShowCreateButton()
        {
            HideButtons();
            CreateButton.gameObject.SetActive(true);
        }

        private void HideButtons()
        {
            JoinButton.gameObject.SetActive(false);
            CreateButton.gameObject.SetActive(false);
        }
    }
}