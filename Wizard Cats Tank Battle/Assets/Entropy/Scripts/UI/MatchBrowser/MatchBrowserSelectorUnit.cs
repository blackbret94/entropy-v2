using Photon.Realtime;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.MatchBrowser
{
    public class MatchBrowserSelectorUnit : MonoBehaviour
    {
        public Button Button;
        public Image Image;
        public TextMeshProUGUI Text;

        private string _roomName = "";
        
        public void InitUnit(RoomInfo room)
        {
            if (room == null) return;
            
            // Get strings
            _roomName = room.Name;
            string roomString = StringifyRoom(_roomName, room);

            if (roomString == null) return;

            Text.text = roomString;

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
            if (_roomName == "")
            {
                Debug.LogError("Cannot join room without name!");
                return;
            }
            
            UIMain.GetInstance().JoinRoom(_roomName);
        }

        private void ShowButton(bool show)
        {
            Button.gameObject.SetActive(show);
        }

        private string StringifyRoom(string roomName, RoomInfo room)
        {
            if (!room.CustomProperties.ContainsKey("map"))
            {
                Debug.LogError("Retrieved lobby that is missing a map!");
                return null;
            }

            string map = (string)room.CustomProperties["map"];
            
            if (!room.CustomProperties.ContainsKey("mode"))
            {
                Debug.LogError("Retrieved lobby that is missing a mode!");
                return null;
            }

            return $"{roomName} | {map} ({room.PlayerCount}/{room.MaxPlayers})";
        }
    }
}