using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Quickplay
{
    public class QuickplayPanel : GamePanel
    {
        public QuickplayPanelMap QuickplayPanelMap;
        public QuickplayPanelMode QuickplayPanelMode;

        public override void OpenPanel()
        {
            base.OpenPanel();
            OpenMode();
        }

        public void OpenMode()
        {
            QuickplayPanelMap.SetActive(false);
            QuickplayPanelMode.SetActive(true);
        }

        public void OpenMap(GameModeDefinition gameMode)
        {
            QuickplayPanelMap.SetActive(true);
            QuickplayPanelMode.SetActive(false);
            QuickplayPanelMap.Inflate(gameMode);
        }
    }
}