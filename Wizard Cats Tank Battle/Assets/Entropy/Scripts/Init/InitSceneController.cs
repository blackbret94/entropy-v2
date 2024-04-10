using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Vashta.Entropy.SceneNavigation;
using Vashta.Entropy.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

namespace Vashta.Entropy.Scripts.Init
{
    public class InitSceneController : MonoBehaviour
    {
        public SceneNavigator SceneNavigator;
        public InitProgressDisplay InitProgressDisplay;
        public List<AddressableDefinition> BundlesToDownloadDef;
        public TextMeshProUGUI errorText;
        public GameObject ConnectionErrorPanel;

        private string _testUrl = "www.google.com";

        private void Start()
        {
            errorText.gameObject.SetActive(false);
            StartCoroutine(CheckInternetConnection());

            StartCoroutine(DownloadAssetsForLabels(BundlesToDownloadDef));
        }
        
        private IEnumerator DownloadAssetsForLabels(List<AddressableDefinition> addressables)
        {
            InitProgressDisplay.SetMaxIndex(addressables.Count);
            
            int i = 0;
            foreach (AddressableDefinition addressable in addressables)
            {
                Debug.Log($"Starting download for label: {addressable.Name}");
                InitProgressDisplay.SetCurrentAssetIndex(i);

                if (addressable.IsScene)
                {
                    yield return DownloadAddressable<SceneInstance>(addressable);
                }
                else
                {
                    yield return DownloadAddressable<Object>(addressable);
                }
                
                i++;
            }

            Debug.Log("All downloads completed.");
            SceneNavigator.GoToLogin();
        }

        private IEnumerator DownloadAddressable<T>(AddressableDefinition addressable)
        {
            var handle = Addressables.LoadAssetsAsync<T>(addressable.Name, null);

            while (!handle.IsDone)
            {
                InitProgressDisplay.UpdateLoadingBar(handle.PercentComplete);
                yield return null; // Wait until the next frame
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Assets for label {addressable.Name} downloaded successfully.");
                InitProgressDisplay.UpdateLoadingBar(1f);
            }
            else
            {
                Debug.LogError($"Failed to download assets for label {addressable.Name}.");
                errorText.gameObject.SetActive(true);
                errorText.text = "There was an error downloading bundle: " + addressable.Name;
            }

            // Addressables.Release(handle);
        }

        private IEnumerator CheckInternetConnection(){
            WWW www = new WWW(_testUrl);
            yield return www;
            if (www.error != null) {
                ConnectionErrorPanel.SetActive(true);
            } else {
                ConnectionErrorPanel.SetActive(false);
            }
        } 
    }

    [System.Serializable]
    public struct AddressableDefinition
    {
        public string Name;
        public bool IsScene;
    }
}