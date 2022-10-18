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
            MainMenuParent.SetActive(true);
            CharacterEditorParent.SetActive(false);
            ItemStoreParent.SetActive(false);
            
            MainMenuGamePanel.Refresh();
        }

        public void GoToCharacterEditor()
        {
            MainMenuParent.SetActive(false);
            CharacterEditorParent.SetActive(true);
            ItemStoreParent.SetActive(false);
            
            CharacterEditorGamePanel.Refresh();
        }

        public void GoToItemStore()
        {
            MainMenuParent.SetActive(false);
            CharacterEditorParent.SetActive(false);
            ItemStoreParent.SetActive(true);
            
            ItemStoreGamePanel.Refresh();
        }
    }
}