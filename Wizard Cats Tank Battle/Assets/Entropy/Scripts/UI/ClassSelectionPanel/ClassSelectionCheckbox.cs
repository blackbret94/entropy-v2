using Entropy.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionCheckbox : MonoBehaviour
    {
        private int _uiIndex;
        
        public Image ClassIcon;
        public Image SelectedFrame;
        public ClassDefinition ClassDefinition;
        public bool StartSelected = false;
        
        private void Start()
        {
            if(ClassDefinition != null)
                ClassIcon.sprite = ClassDefinition.classIcon;
            
            Toggle(StartSelected);
        }
        
        public void Toggle(bool setActive)
        {
            SelectedFrame.gameObject.SetActive(setActive);
        }

        public void SetUI_Index(int newIndex)
        {
            _uiIndex = newIndex;
        }

        public int GetUI_Index()
        {
            return _uiIndex;
        }
    }
}