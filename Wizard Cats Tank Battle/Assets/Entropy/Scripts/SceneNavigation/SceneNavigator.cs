using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vashta.Entropy.SceneNavigation
{
    public class SceneNavigator: MonoBehaviour
    {
        private const string WardrobeSceneName = "CharacterEditor";
        private const string MainMenuSceneName = "Intro";
        private const string ItemStoreSceneName = "Store";
        
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }

        public void GoToWardrobe()
        {
            SceneManager.LoadScene(WardrobeSceneName);
        }

        public void GoToItemStore()
        {
            SceneManager.LoadScene(ItemStoreSceneName);
        }
    }
}