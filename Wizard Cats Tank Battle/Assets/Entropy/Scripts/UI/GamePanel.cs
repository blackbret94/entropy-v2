using UnityEngine;
using System.Collections.Generic;
using Entropy.Scripts.Player;
using TanksMP;
using UnityEngine.Serialization;

namespace Vashta.Entropy.UI
{
    public class GamePanel: MonoBehaviour
    {
        public List<GamePanel> ChildPanels;
        // Does hitting "escape" close this panel?
        public bool CanBeClosedByHotkey = false;
        // Should this stop gameplay actions such as movement?
        [FormerlySerializedAs("OverrideGameMovementWhenSelected")] public bool BlockGameplayWhenSelected = false;
        
        public virtual void Refresh()
        {
            foreach (GamePanel panel in ChildPanels)
            {
                panel.Refresh();
            }
        }

        public virtual void TogglePanel()
        {
            if(gameObject.activeSelf)
                ClosePanel();
            else
                OpenPanel();
        }
        
        public virtual void SetActive(bool b)
        {
            gameObject.SetActive(b);
        }
        
        public virtual void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        public virtual void ClosePanel()
        {
            gameObject.SetActive(false);
        }

        public virtual void CloseByHotkey()
        {
            if(CanBeClosedByHotkey)
                ClosePanel();
        }

        public virtual void UI_Up()
        {
            
        }

        public virtual void UI_Down()
        {
            
        }

        public virtual void UI_Left()
        {
            
        }

        public virtual void UI_Right()
        {
            
        }

        public void SetAsSelectedPanel()
        {
            UIGame uiGame = UIGame.GetInstance();
            if (uiGame)
            {
                PlayerInputController playerInputController = uiGame.PlayerInputController;

                if (playerInputController)
                {
                    playerInputController.SetSelectedPanel(this);
                }
            }
            
            // TODO: UIMain
        }
    }
}