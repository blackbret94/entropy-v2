using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vashta.Entropy.SceneNavigation
{
    public class SceneNavigator: MonoBehaviour
    {
        private const string WardrobeSceneName = "CharacterEditor";
        private const string MainMenuSceneName = "MainMenu";
        private const string ItemStoreSceneName = "Store";
        private const string LoginSceneName = "Login";
        private const string InitSceneName = "Init";
        
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }

        public void GoToMainMenuAsync()
        {
            StartCoroutine(LoadMainMenuAsync());
        }
        
        IEnumerator LoadMainMenuAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(MainMenuSceneName);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        public void GoToWardrobe()
        {
            SceneManager.LoadScene(WardrobeSceneName);
        }

        public void GoToItemStore()
        {
            SceneManager.LoadScene(ItemStoreSceneName);
        }

        public void GoToLogin()
        {
            SceneManager.LoadScene(LoginSceneName);
        }

        public void GoToInit()
        {
            SceneManager.LoadScene(InitSceneName);
        }
    }
}