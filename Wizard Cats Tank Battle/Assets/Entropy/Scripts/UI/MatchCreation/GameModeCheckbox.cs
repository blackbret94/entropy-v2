using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class GameModeCheckbox : MonoBehaviour
    {
        public GameModeDefinition GameModeDefinition;

        public Image GameModeImage;
        public Image SelectedFrame;
        public bool StartSelected;
        public bool IsRandom;

        private void Start()
        {
            if (GameModeDefinition != null)
            {
                GameModeImage.sprite = GameModeDefinition.Icon;
            }
            
            Toggle(StartSelected);
        }
        
        public void Toggle(bool b)
        {
            SelectedFrame.gameObject.SetActive(b);
        }
    }
}