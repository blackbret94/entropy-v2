using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI.Hotkey
{
    public class HotkeyManagerUI : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetButtonDown("CloseWindow")) CloseWindow();
            if(Input.GetButtonDown("ToggleClassSelection")) ToggleClassSelectionPanel();
            if(Input.GetButtonDown("ToggleSettings")) ToggleSettings();
            if(Input.GetButtonDown("ToggleScoreboard")) ToggleLeaderboard();
        }

        private void CloseWindow()
        {
            GamePanel[] gamePanels = FindObjectsByType<GamePanel>(FindObjectsSortMode.None);

            foreach (GamePanel gamePanel in gamePanels)
            {
                gamePanel.CloseByHotkey();
            }
        }

        private void ToggleClassSelectionPanel()
        {
            if (UIGame.GetInstance().ClassSelectionPanel.isActiveAndEnabled)
            {
                CloseWindow();
            }
            else
            {
                CloseWindow();
                UIGame.GetInstance().ClassSelectionPanel.TogglePanel();
            }
        }

        private void ToggleSettings()
        {
            if (UIGame.GetInstance().SettingsPanel.isActiveAndEnabled)
            {
                CloseWindow();
            }
            else
            {
                CloseWindow();
                UIGame.GetInstance().SettingsPanel.TogglePanel();
            }
        }

        private void ToggleLeaderboard()
        {
            if (UIGame.GetInstance().ScoreboardPanel.isActiveAndEnabled)
            {
                CloseWindow();
            }
            else
            {
                CloseWindow();
                UIGame.GetInstance().ScoreboardPanel.TogglePanel();
            }
        }

        private void ToggleThroughInputs()
        {
            
        }
    }
}