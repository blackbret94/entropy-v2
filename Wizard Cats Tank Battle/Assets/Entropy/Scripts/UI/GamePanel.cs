using UnityEngine;
using System.Collections.Generic;

namespace Vashta.Entropy.UI
{
    public class GamePanel: MonoBehaviour
    {
        public List<GamePanel> ChildPanels;
        // Does hitting "escape" close this panel?
        public bool CanBeClosedByHotkey = false;
        
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
    }
}