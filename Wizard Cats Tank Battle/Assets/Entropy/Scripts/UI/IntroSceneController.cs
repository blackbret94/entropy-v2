using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class IntroSceneController: MonoBehaviour
    {
        public GameObject MainMenuParent,
            CharacterEditorParent;

        public GamePanel MainMenuGamePanel,
            CharacterEditorGamePanel;

        public void GoToMainMenu()
        {
            DisableAll();
            
            MainMenuParent.SetActive(true);
            MainMenuGamePanel.SetActive(true);
            MainMenuGamePanel.Refresh();
        }

        public void GoToCharacterEditor()
        {
            DisableAll();
            
            CharacterEditorParent.SetActive(true);
            CharacterEditorGamePanel.SetActive(true);
            CharacterEditorGamePanel.Refresh();
        }
        
        private void DisableAll()
        {
            MainMenuParent.SetActive(false);
            CharacterEditorParent.SetActive(false);
            
            MainMenuGamePanel.SetActive(false);
            CharacterEditorGamePanel.SetActive(false);
        }
    }
}