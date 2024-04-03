using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Vashta.Entropy.SceneNavigation
{
    public class SceneNavigator: MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";
        private const string LoginSceneName = "Login";
        private const string InitSceneName = "Init";
        
        public void GoToMainMenu()
        {
            Addressables.LoadSceneAsync(MainMenuSceneName);
        }

        public void GoToMainMenuAsync()
        {
            StartCoroutine(LoadMainMenuAsync());
        }
        
        IEnumerator LoadMainMenuAsync()
        {
            UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<Object>("entropy-maps-01", delegate(Object o)
            {
                var asyncLoad = Addressables.LoadSceneAsync(MainMenuSceneName);

                // Wait until the asynchronous scene fully loads
                // while (!asyncLoad.IsDone)
                // {
                //     yield return null;
                // }
            });
            yield return null;
        }
        
        public void GoToLogin()
        {
            Addressables.LoadSceneAsync(LoginSceneName);
        }

        public void GoToInit()
        {
            Addressables.LoadSceneAsync(InitSceneName);
        }
    }
}