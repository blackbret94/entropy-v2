namespace Vashta.Entropy.UI
{
    public class SettingsPanel : GamePanel
    {
        public override void OpenPanel()
        {
            base.OpenPanel();
            HUDPanel.Get().ClosePanel();
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
            HUDPanel.Get().OpenPanel();
        }
    }
}