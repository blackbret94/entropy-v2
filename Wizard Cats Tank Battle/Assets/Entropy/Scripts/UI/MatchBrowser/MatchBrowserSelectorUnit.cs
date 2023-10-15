using Photon.Realtime;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.MatchBrowser
{
    public class MatchBrowserSelectorUnit : MonoBehaviour
    {
        public Button Button;
        public Image Image;
        public TextMeshProUGUI Text;
        public Image GameModeIcon;

        public GameModeDictionary GameModeDictionary;

        private string _roomName = "";
        private RoomInfoWrapper _roomInfoWrapper;
        
        public void InitUnit(RoomInfo room)
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
            ShowButton(room.IsOpen);
        }

        /// <summary>
        /// Display placeholder text if no rooms are found
        /// </summary>
        public void SetNoRoomsFound()
        {
            Text.text = "No matches could be found for the selected region!";
            ShowButton(false);
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
            
            UIMain.GetInstance().JoinRoom(roomNameId);
        }

        private void ShowButton(bool show)
        {
            Button.gameObject.SetActive(show);
        }
    }
}