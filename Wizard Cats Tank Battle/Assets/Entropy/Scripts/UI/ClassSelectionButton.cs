using Entropy.Scripts.Player;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class ClassSelectionButton : MonoBehaviour
    {
        public Image Icon;

        public void UpdateIcon()
        {
            if (!Icon)
                return;
            
            ClassDefinition playerClass = GameManager.GetInstance().localPlayerController.GetClass();

            if (!playerClass)
                return;
            
            Icon.sprite = playerClass.classIcon;
        }

    }
}