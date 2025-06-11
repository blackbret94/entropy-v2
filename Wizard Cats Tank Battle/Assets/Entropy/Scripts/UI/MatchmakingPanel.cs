using TMPro;
using UnityEngine;
using Vashta.Entropy.UI.MatchBrowser;
using Vashta.Entropy.UI.MatchCreation;
using Vashta.Entropy.UI.Quickplay;

namespace Vashta.Entropy.UI
{
    public class MatchmakingPanel : GamePanel
    {
        public MatchBrowserPanel BrowsePanel;
        public MatchCreationPanel CreatePanel;
        public QuickplayPanel QuickplayPanel;
        
        public TextMeshProUGUI BrowseButtonText;
        public TextMeshProUGUI CreateButtonText;
        public TextMeshProUGUI PracticeButtonText;
        public TextMeshProUGUI MatchmakingButtonText;

        public Color UnselectedColor;
        public Color SelectedColor;
        
        private void HideAll()
        {
            BrowsePanel.ClosePanel();
            CreatePanel.ClosePanel();
            QuickplayPanel.ClosePanel();

            BrowseButtonText.color = UnselectedColor;
            CreateButtonText.color = UnselectedColor;
            PracticeButtonText.color = UnselectedColor;
            MatchmakingButtonText.color = UnselectedColor;
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
            CreatePanel.ToggleMultiplayer(MatchPanelType.Create);
            CreateButtonText.color = SelectedColor;
        }
        
        public void ShowMatchmaking()
        {
            OpenPanel();
            HideAll();
            
            QuickplayPanel.OpenPanel();
            MatchmakingButtonText.color = SelectedColor;
        }

        public void ShowPracticePanel()
        {
            OpenPanel();
            HideAll();
            
            CreatePanel.OpenPanel();
            CreatePanel.ToggleMultiplayer(MatchPanelType.Practice);
            PracticeButtonText.color = SelectedColor;
        }
    }
}