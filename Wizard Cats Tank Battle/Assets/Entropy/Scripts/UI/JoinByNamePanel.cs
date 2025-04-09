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
            string roomPassword = RoomPasswordInput.text;
        }
    }
}