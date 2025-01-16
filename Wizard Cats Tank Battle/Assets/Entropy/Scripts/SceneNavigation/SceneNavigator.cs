using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vashta.Entropy.SceneNavigation
{
    public class SceneNavigator: NetworkBehaviour
    {
        private const string SceneBasePath = "Assets/Entropy/Scenes/";
        private const string MainMenuSceneName = "MainMenu";
        private const string LoginSceneName = "Login";
        private const string InitSceneName = "Init";
        
        public void GoToMainMenu()
        {
            Runner.LoadScene(GetSceneRef(MainMenuSceneName));
        }
        
        public void GoToLogin()
        {
            Runner.LoadScene(GetSceneRef(LoginSceneName));
        }

        public void GoToInit()
        {
            Runner.LoadScene(GetSceneRef(InitSceneName));
        }

        public void GoToScene(string sceneName)
        {
            Runner.LoadScene(GetSceneRef(sceneName));
        }

        private SceneRef GetSceneRef(string sceneName)
        {
            return SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(SceneBasePath+sceneName));
        }
        
        public bool IsMainMenu()
        {
            return SceneManager.GetActiveScene().name == MainMenuSceneName;
        }
    }
}