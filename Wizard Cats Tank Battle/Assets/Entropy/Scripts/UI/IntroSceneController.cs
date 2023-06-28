using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class IntroSceneController: MonoBehaviour
    {
        public GameObject MainMenuParent,
            CharacterEditorParent,
            ItemStoreParent;

        public GamePanel MainMenuGamePanel,
            CharacterEditorGamePanel,
            ItemStoreGamePanel;

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

        public void GoToItemStore()
        {
            DisableAll();
            
            ItemStoreParent.SetActive(true);
            ItemStoreGamePanel.SetActive(true);
            ItemStoreGamePanel.Refresh();
        }

        private void DisableAll()
        {
            MainMenuParent.SetActive(false);
            CharacterEditorParent.SetActive(false);
            ItemStoreParent.SetActive(false);
            
            MainMenuGamePanel.SetActive(false);
            CharacterEditorGamePanel.SetActive(false);
            ItemStoreGamePanel.SetActive(false);
        }
    }
}