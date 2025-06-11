using Fusion;
using TanksMP;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Quickplay
{
    public class QuickplayPanelMode : GamePanel
    {
        public QuickplayPanel QuickplayPanel;
        public RoomOptionsFactory RoomOptionsFactory;
        
        public void StartQuickplay()
        {
            // Start session random map + random mode
            StartGameArgs startGameArgs = RoomOptionsFactory.QuickplayArgs();
            UIMain.GetInstance().roomConnectionController.CreateRoom(startGameArgs);
        }

        public void StartGameMode(GameModeDefinition mode)
        {
            // Go to game panel for maps
            QuickplayPanel.OpenMap(mode);
        }
    }
}