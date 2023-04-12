using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    // Should handle teams as well as autoassign
    public class ClassSelectionTeamCheckbox : MonoBehaviour
    {
        public int TeamIndex;
        public bool IsAutoAssign;
        public Image SelectedFrame;
        public bool StartSelected = false;

        public const int AUTO_ASSIGN_INDEX = -1;

        private void Start()
        {
            Toggle(StartSelected);
        }
        
        public void Toggle(bool setActive)
        {
            SelectedFrame.gameObject.SetActive(setActive);
        }
    }
}