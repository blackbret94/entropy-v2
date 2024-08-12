using TanksMP;
using UnityEngine;
using Vashta.Entropy.SaveLoad;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.Player
{
    public abstract class PlayerInputAdapter
    {
        protected InputDirectory InputDirectory;
        protected PlayerInputController PlayerInputController;

        public PlayerInputAdapter(InputDirectory inputDirectory, PlayerInputController playerInputController)
        {
            InputDirectory = inputDirectory;
            PlayerInputController = playerInputController;
        }

        public void Update()
        {
            // Gameplay actions
            if(ShouldDropCollectible()) DropSpoon();
            if(ShouldUseUltimate()) CastUltimate();
            if(ShouldUsePowerup()) CastPowerup();
            
            // UI Actions
            if(ShouldClosePanel()) CloseWindow();
            if(ShouldToggleClassSelection()) ToggleClassSelectionPanel();
            if(ShouldToggleSettings()) ToggleSettings();
            if(ShouldToggleScoreboard()) ToggleScoreboard();
            
            // UI Directions
            if(DetectUI_Up()) UI_Up();
            if(DetectUI_Down()) UI_Down();
            if(DetectUI_Left()) UI_Left();
            if(DetectUI_Right()) UI_Right();
        }
        
        // Gameplay
        public abstract Vector2 GetMovementVector(out bool isMoving);
        public abstract Vector2 GetTurretRotation(Vector3 pos);
        public abstract bool ShouldShoot();
        public abstract bool ShouldDropCollectible();
        public abstract bool ShouldUsePowerup();
        public abstract bool ShouldUseUltimate();
        
        // UI
        public abstract bool ShouldToggleSettings();
        public abstract bool ShouldToggleClassSelection();
        public abstract bool ShouldToggleScoreboard();
        public abstract bool ShouldClosePanel();
        
        // UI Directions
        public abstract bool DetectUI_Up();
        public abstract bool DetectUI_Down();
        public abstract bool DetectUI_Left();
        public abstract bool DetectUI_Right();
        
        protected void DropSpoon()
        {
            // drop spoon
            TanksMP.Player player = TanksMP.Player.GetLocalPlayer();
            
            if (player != null)
            {
                player.CommandDropCollectibles();
                UIGame.GetInstance().DropCollectiblesButton.gameObject.SetActive(false);
            }
        }

        protected void CastUltimate()
        {
            TanksMP.Player player = TanksMP.Player.GetLocalPlayer();
            
            if (player != null)
            {
                bool couldCast = player.TryCastUltimate();
            
                if (!couldCast)
                    GameManager.GetInstance().ui.SfxController.PlayUltimateNotReady();
            }
        }

        protected void CastPowerup()
        {
            TanksMP.Player player = TanksMP.Player.GetLocalPlayer();

            if (player != null)
            {
                player.TryCastPowerup();
            }
        }
        
        protected void CloseWindow()
        {
            GamePanel[] gamePanels = Object.FindObjectsByType<GamePanel>(FindObjectsSortMode.None);

            foreach (GamePanel gamePanel in gamePanels)
            {
                gamePanel.CloseByHotkey();
            }
        }

        protected void ToggleClassSelectionPanel()
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

        protected void ToggleSettings()
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

        protected void ToggleScoreboard()
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

        protected void UI_Up()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Up();
            }
        }

        protected void UI_Down()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Down();
            }
        }

        protected void UI_Left()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Left();
            }
        }

        protected void UI_Right()
        {
            GamePanel selectedPanel = PlayerInputController.GetSelectedGamePanel();
            if (selectedPanel)
            {
                selectedPanel.UI_Right();
            }
        }
    }
}