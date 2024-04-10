using UnityEngine;

namespace Vashta.Entropy.SceneNavigation
{
    [RequireComponent(typeof(AddressableSceneManager))]
    public class SceneNavigator: MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";
        private const string LoginSceneName = "Login";
        private const string InitSceneName = "Init";

        private AddressableSceneManager _addressableSceneManager;

        private AddressableSceneManager GetSceneManager()
        {
            if (!_addressableSceneManager)
                _addressableSceneManager = GetComponent<AddressableSceneManager>();

            if (!_addressableSceneManager)
            {
                Debug.LogError("A SceneNavigator is missing an AddressableSceneManager component!");
            }

            return _addressableSceneManager;
        }
        
        public void GoToMainMenu()
        {
            GetSceneManager().GoToScene(MainMenuSceneName);
        }
        
        public void GoToLogin()
        {
            GetSceneManager().GoToScene(LoginSceneName);
        }

        public void GoToInit()
        {
            GetSceneManager().GoToScene(InitSceneName);
        }
    }
}