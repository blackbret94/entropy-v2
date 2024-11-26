using TMPro;
using UnityEngine;
using Vashta.Entropy.UI.MatchBrowser;
using Vashta.Entropy.UI.MatchCreation;

namespace Vashta.Entropy.UI
{
    public class MatchmakingPanel : GamePanel
    {
        public MatchBrowserPanel BrowsePanel;
        public MatchCreationPanel CreatePanel;
        
        public TextMeshProUGUI BrowseButtonText;
        public TextMeshProUGUI CreateButtonText;
        public TextMeshProUGUI PracticeButtonText;

        public Color UnselectedColor;
        public Color SelectedColor;
        
        private void HideAll()
        {
            BrowsePanel.ClosePanel();
            CreatePanel.ClosePanel();

            BrowseButtonText.color = UnselectedColor;
            CreateButtonText.color = UnselectedColor;
            PracticeButtonText.color = UnselectedColor;
        }

        public void ShowBrowse()
        {
            OpenPanel();
            HideAll();
            
            BrowsePanel.OpenPanel();
            BrowseButtonText.color = SelectedColor;
        }

        public void ShowCreate()
        {
            OpenPanel();
            HideAll();
            
            CreatePanel.OpenPanel();
            CreatePanel.ToggleMultiplayer(true);
            CreateButtonText.color = SelectedColor;
        }

        public void ShowPracticePanel()
        {
            OpenPanel();
            HideAll();
            
            CreatePanel.OpenPanel();
            CreatePanel.ToggleMultiplayer(false);
            PracticeButtonText.color = SelectedColor;
        }
    }
}