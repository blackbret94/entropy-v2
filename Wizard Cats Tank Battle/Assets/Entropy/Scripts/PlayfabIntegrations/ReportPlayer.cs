using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Vashta.Entropy.UI.ReportPlayerPanel;

namespace Vashta.Entropy.Scripts.PlayfabIntegrations
{
    public class ReportPlayer
    {
        private ReportPlayerPanel _panel;

        public ReportPlayer(ReportPlayerPanel panel)
        {
            _panel = panel;
        }
        
        public void Report(string reporteePlayfabId, string reason, string comment = "")
        {
            ReportPlayerClientRequest request = new ReportPlayerClientRequest
            {
                ReporteeId = reporteePlayfabId,
                CustomTags = new Dictionary<string, string> { { "Reason", reason } },
                Comment = comment
            };

            PlayFabClientAPI.ReportPlayer(
                request,
                OnReportSuccess,
                OnReportFailure
            );
        }

        private void OnReportSuccess(ReportPlayerClientResult result)
        {
            if (_panel != null)
            {
                _panel.SetConfirmationMessage($"Report submitted successfully!", true);
            }
            Debug.Log($"Report submitted successfully. Reports remaining: {result.SubmissionsRemaining}");
        }

        private void OnReportFailure(PlayFabError error)
        {
            if (_panel != null)
            {
                _panel.SetConfirmationMessage($"Could not report player! Try again later.", false);
            }
            
            Debug.LogError($"Error reporting player: {error.GenerateErrorReport()}");
        }
    }
}