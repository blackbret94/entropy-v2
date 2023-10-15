using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.UI.MapSelection;
using Vashta.Entropy.UI.MatchBrowser;
using Vashta.Entropy.UI.MatchCreation;

namespace Vashta.Entropy.UI
{
    public class MatchmakingPanel : GamePanel
    {
        public QuickplayPanel QuickplayPanel;
        public MatchBrowserPanel BrowsePanel;
        public MatchCreationPanel CreatePanel;
        public PracticePanel PracticePanel;

        public TextMeshProUGUI QuickplayButtonText;
        public TextMeshProUGUI BrowseButtonText;
        public TextMeshProUGUI CreateButtonText;
        public TextMeshProUGUI PracticeButtonText;

        public Color UnselectedColor;
        public Color SelectedColor;
        
        private void HideAll()
        {
            QuickplayPanel.ClosePanel();
            BrowsePanel.ClosePanel();
            CreatePanel.ClosePanel();
            PracticePanel.ClosePanel();

            QuickplayButtonText.color = UnselectedColor;
            BrowseButtonText.color = UnselectedColor;
            CreateButtonText.color = UnselectedColor;
            PracticeButtonText.color = UnselectedColor;
        }

        public void ShowQuickplay()
        {
            OpenPanel();
            HideAll();
            
            QuickplayPanel.OpenPanel();
            QuickplayButtonText.color = SelectedColor;
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
            CreateButtonText.color = SelectedColor;
        }

        public void ShowPracticePanel()
        {
            OpenPanel();
            HideAll();
            
            PracticePanel.OpenPanel();
            PracticeButtonText.color = SelectedColor;
        }
    }
}