using System.Collections.Generic;
using Entropy.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.Player;
using Vashta.Entropy.Scripts.PlayfabIntegrations;

namespace Vashta.Entropy.UI.ReportPlayerPanel
{
    public class ReportPlayerPanel : GamePanel
    {
        [FormerlySerializedAs("ErrorMessage")] public TextMeshProUGUI ConfirmationMessage;
        public Color SuccessColor = Color.white;
        public Color ErrorColor = Color.red;
        public TMP_InputField Comments;
        [FormerlySerializedAs("Dropdown")] public TMP_Dropdown ReasonDropdown;
        public TMP_Dropdown PlayerDropdown;
        
        private List<PlayerReportInfo> _playerReports = new();
        private ReportPlayer _reportPlayer;

        private void Awake()
        {
            _reportPlayer = new ReportPlayer(this);
        }

        public void Submit()
        {
            // validate form
            if (PlayerDropdown.value == 0)
            {
                SetConfirmationMessage("Could not submit: Please select a player", false);
                return;
            }
            
            // get player
            PlayerReportInfo playerReportInfo = _playerReports[PlayerDropdown.value];

            if (string.IsNullOrEmpty(playerReportInfo.PlayfabId))
            {
                SetConfirmationMessage("Could not submit: Selected player is invalid", false);
            }
            
            // attempt to report
            _reportPlayer.Report(playerReportInfo.PlayfabId, ReasonDropdown.options[ReasonDropdown.value].text, Comments.text);
            SetConfirmationMessage("Submitting report...", true);
        }
        
        public override void OpenPanel()
        {
            base.OpenPanel();
            
            ConfirmationMessage.text = "";
            Comments.text = "";
            ReasonDropdown.value = 0;
            
            // Clear dropdown
            PlayerDropdown.ClearOptions();
            PlayerDropdown.value = 0;
            
            // add players
            _playerReports.Clear();
            List<string> dropdownOptions = new();
            
            // add "Select a player" option so no players are "default" selected
            _playerReports.Add(new PlayerReportInfo("Select A Player", ""));
            dropdownOptions.Add("Select A Player");
            
            // Get all players
            List<PlayerController> players = PlayerList.GetAllPlayers;

            foreach (PlayerController playerController in players)
            {
                // ignore if local player or bot
                if(playerController.IsLocal || playerController.isBot)
                    continue;
                
                _playerReports.Add(new PlayerReportInfo(playerController.PlayerName, playerController.playfabId));
                dropdownOptions.Add(playerController.PlayerName);
            }

            // populate
            PlayerDropdown.AddOptions(dropdownOptions);
        }

        public void SetConfirmationMessage(string text, bool success)
        {
            ConfirmationMessage.text = text;
            ConfirmationMessage.color = success ? SuccessColor : ErrorColor;
        }
    }
}