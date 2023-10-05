using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class MatchmakingPanel : GamePanel
    {
        public GameObject QuickplayPanel;
        public GameObject BrowsePanel;
        public GameObject CreatePanel;

        private void Start()
        {
            ShowQuickplay();
        }
        
        private void HideAll()
        {
            QuickplayPanel.SetActive(false);
            BrowsePanel.SetActive(false);
            CreatePanel.SetActive(false);
        }

        public void ShowQuickplay()
        {
            HideAll();
            QuickplayPanel.SetActive(true);
        }

        public void ShowBrowse()
        {
            HideAll();
            BrowsePanel.SetActive(true);
        }

        public void ShowCreate()
        {
            HideAll();
            CreatePanel.SetActive(true);
        }
    }
}