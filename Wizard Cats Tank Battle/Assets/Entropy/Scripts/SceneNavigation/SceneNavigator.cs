using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

namespace Vashta.Entropy.SceneNavigation
{
    public class SceneNavigator: MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";
        private const string LoginSceneName = "Login";
        private const string InitSceneName = "Init";

        private List<AsyncOperationHandle<SceneInstance>> SceneHandles;
        
        public void GoToMainMenu()
        {
            GoToScene(MainMenuSceneName);
        }
        
        public void GoToLogin()
        {
            GoToScene(LoginSceneName);
        }

        public void GoToInit()
        {
            GoToScene(InitSceneName);
        }

        public void OnDestroy()
        {
            foreach (var handle in SceneHandles)
            {
                Addressables.Release(handle);
            }
        }

        public void GoToScene(string key)
        {
            Addressables.LoadAssetsAsync<Object>("entropy-maps-01", delegate(Object o)
            {
                var handle = Addressables.LoadSceneAsync(key);
                RegisterHandle(handle);
            });
        }

        private void RegisterHandle(AsyncOperationHandle<SceneInstance> handle)
        {
            if (SceneHandles == null)
                SceneHandles = new List<AsyncOperationHandle<SceneInstance>>();
            
            SceneHandles.Add(handle);
        }
    }
}