using UnityEngine;
using System.Collections.Generic;

namespace Vashta.Entropy.UI
{
    public class GamePanel: MonoBehaviour
    {
        public List<GamePanel> ChildPanels;
        
        public virtual void Refresh()
        {
            foreach (GamePanel panel in ChildPanels)
            {
                panel.Refresh();
            }
        }
    }
}