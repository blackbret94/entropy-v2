using TanksMP;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class JoinByNamePanel : GamePanel
    {
        public InputField RoomNameInput;
        public InputField RoomPasswordInput;

        public void JoinByRoomName()
        {
            string roomName = RoomNameInput.text;
            
            NetworkManagerCustom networkManagerCustom = NetworkManagerCustom.GetInstance();
            if (networkManagerCustom != null)
            {
                networkManagerCustom.JoinRoom(roomName);
            }
            else
            {
                Debug.LogError("Could not find NetworkManagerCustom!");
            }
        }
    }
}