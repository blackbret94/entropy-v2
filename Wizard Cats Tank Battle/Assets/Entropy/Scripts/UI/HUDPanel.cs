namespace Vashta.Entropy.UI
{
    public class HUDPanel : GamePanel
    {
        private static HUDPanel _instance;

        public static HUDPanel Get() => _instance;
        
        private void Start()
        {
            _instance = this;
            ClosePanel();
        }

    }
}