using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionPanel : GamePanel
    {
        public ClassSelectionSelectorMultipanel ClassSelectionSelectorMultipanel;
        public ClassSelectionTeamSelector ClassSelectionTeamSelector;
        public List<ClassSelectionTeamCheckbox> CheckboxList;

        public float TimerLength = 30f;
        public TextMeshProUGUI Counter;

        public GameObject ApplyButton,
            ApplyButtonLow;
        public GameObject RespawnButton,
            RespawnButtonLow;
        public GameObject CountdownSelectionButton,
            CountdownSelectionButtonLow;
        
        private float _counterEndTime;
        private CanvasGroup _canvasGroup;

        private static ClassSelectionPanel _instance;

        public static ClassSelectionPanel Instance => _instance;
        
        private void Start()
        {
            _instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1;
            
            ShowFreeRespawnButtons();
            _counterEndTime = Time.time + TimerLength;

            StartCoroutine(RefreshCheckboxesAtStart());
        }

        private IEnumerator RefreshCheckboxesAtStart()
        {
            yield return new WaitForSeconds(.25f);

            UpdateCheckboxes();
        }

        private void UpdateCheckboxes()
        {
            // update top selection
            Player player = GameManager.GetInstance().localPlayer;
            int teamIndex = player.PreferredTeamIndex;

            foreach (ClassSelectionTeamCheckbox checkbox in CheckboxList)
            {
                if (checkbox.TeamIndex == teamIndex)
                    ClassSelectionTeamSelector.SelectTeam(checkbox);
            }
        }
        
        public override void OpenPanel()
        {
            UpdateCheckboxes();
            HUDPanel.Get().ClosePanel();
            UpdateButtonsAtBottomOfScreen();

            base.OpenPanel();
            
            SetAsSelectedPanel();
        }

        public override void ClosePanel()
        {
            HUDPanel.Get().OpenPanel();
            base.ClosePanel();
        }

        private void Update()
        {
            
            if (CountdownIsActive())
            {
                Counter.text = Mathf.CeilToInt(_counterEndTime - Time.time).ToString();
            }
            else
            {
                if (Counter.IsActive())
                {
                    Counter.enabled = false;
                    UpdateButtonsAtBottomOfScreen();
                }
            }
        }

        private void UpdateButtonsAtBottomOfScreen()
        {
            if (CountdownIsActive() || IsInRespawnZone())
            {
                ShowFreeRespawnButtons();
            }
            else
            {
                ShowApplyRespawnButtons();
            }
        }

        /// <summary>
        /// These buttons are shown when the player can respawn freely
        /// </summary>
        private void ShowFreeRespawnButtons()
        {
            CountdownSelectionButton.SetActive(true);
            CountdownSelectionButtonLow.SetActive(true);
            
            RespawnButton.SetActive(false);
            RespawnButtonLow.SetActive(false);
            
            ApplyButton.SetActive(false);
            ApplyButtonLow.SetActive(false);
        }

        /// <summary>
        /// These buttons are shown when the player cannot respawn freely
        /// </summary>
        private void ShowApplyRespawnButtons()
        {
            CountdownSelectionButton.SetActive(false);
            CountdownSelectionButtonLow.SetActive(false);
            
            RespawnButton.SetActive(true);
            RespawnButtonLow.SetActive(true);
            
            ApplyButton.SetActive(true);
            ApplyButtonLow.SetActive(true);
        }

        public void ApplyChangesButton()
        {
            ApplyChanges(false, false);
        }

        private void ApplyChanges(bool respawnPlayer, bool applyNow)
        {
            Player player = GameManager.GetInstance().localPlayer;
            player.PreferredTeamIndex = ClassSelectionTeamSelector.SelectedTeamIndex();

            player.SetClass(ClassSelectionSelectorMultipanel.SelectedClassDefinition(), respawnPlayer, applyNow);
            player.CmdTryChangeTeams(respawnPlayer);
            ClosePanel();
        }

        public void RespawnPlayerButton()
        {
            ApplyChanges(true, true);
        }

        public void RespawnPlayerIfTeamChangedButton()
        {
            Player player = GameManager.GetInstance().localPlayer;
            int teamIndex = player.TeamIndex;

            bool teamChanged = teamIndex != ClassSelectionTeamSelector.SelectedTeamIndex();
            
            ApplyChanges(teamChanged, true);
        }

        public bool CountdownIsActive()
        {
            return _counterEndTime >= Time.time;
        }

        private bool IsInRespawnZone()
        {
            return GameManager.GetInstance().localPlayer.PlayerCanRespawnFreely();
        }
        
        // UI
        public override void UI_Down()
        {
            GamePanel gamePanel = ClassSelectionSelectorMultipanel.GetPanel();
            ClassSelectionSelector selector = (ClassSelectionSelector)gamePanel;
            selector.SelectNext();
        }

        public override void UI_Up()
        {
            GamePanel gamePanel = ClassSelectionSelectorMultipanel.GetPanel();
            ClassSelectionSelector selector = (ClassSelectionSelector)gamePanel;
            selector.SelectPrevious();
        }

        public override void UI_Primary()
        {
            if (IsInRespawnZone())
            {
                RespawnPlayerButton();
            }
            else
            {
                ApplyChangesButton();
            }
        }

        public override void UI_Tertiary()
        {
            RespawnPlayerButton();
        }
    }
}