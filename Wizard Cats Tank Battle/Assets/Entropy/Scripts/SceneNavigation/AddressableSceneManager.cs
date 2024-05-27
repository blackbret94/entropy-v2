using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Vashta.Entropy.SceneNavigation
{
    public class AddressableSceneManager : MonoBehaviour
    {
        // private List<AsyncOperationHandle<SceneInstance>> SceneHandles;
        private const string MAPS_ASSET_BUNDLE = "entropy-maps-01";
        public const string MAIN_MENU_NAME = "MainMenu";
        public const string LOGIN_NAME = "Login";
        public const string INIT_NAME = "Init";
        
        public void GoToScene(string key)
        {
            // Addressables.LoadAssetsAsync<SceneInstance>(MAPS_ASSET_BUNDLE, delegate(SceneInstance o)
            // {
            SceneManager.LoadScene(key);
            // Addressables.LoadSceneAsync(key);//.Completed += Addressables.Release;
            // RegisterHandle(handle);
            // });
        }
        
        // private void RegisterHandle(AsyncOperationHandle<SceneInstance> handle)
        // {
        //     if (SceneHandles == null)
        //         SceneHandles = new List<AsyncOperationHandle<SceneInstance>>();
        //     
        //     SceneHandles.Add(handle);
        // }
        
        // public void OnDestroy()
        // {
        //     if (SceneHandles != null)
        //     {
        //         foreach (var handle in SceneHandles)
        //         {
        //             Addressables.Release(handle);
        //         }
        //     }
        // }

        public bool IsMainMenu()
        {
            return SceneManager.GetActiveScene().name == MAIN_MENU_NAME;
        }
    }
}