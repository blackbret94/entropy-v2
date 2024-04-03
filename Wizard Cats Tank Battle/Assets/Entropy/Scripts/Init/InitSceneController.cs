using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Vashta.Entropy.SceneNavigation;
using Vashta.Entropy.UI;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Vashta.Entropy.Scripts.Init
{
    public class InitSceneController : MonoBehaviour
    {
        public SceneNavigator SceneNavigator;
        public InitProgressDisplay InitProgressDisplay;
        public List<string> BundlesToDownload;
        public TextMeshProUGUI errorText;
        public GameObject ConnectionErrorPanel;

        private string _testUrl = "www.google.com";

        private void Start()
        {
            errorText.gameObject.SetActive(false);
            StartCoroutine(CheckInternetConnection());

            StartCoroutine(DownloadAssetsForLabels(BundlesToDownload));
        }
        
        private IEnumerator DownloadAssetsForLabels(List<string> labels)
        {
            List<AsyncOperationHandle> handles = new List<AsyncOperationHandle>();

            InitProgressDisplay.SetMaxIndex(labels.Count);
            
            int i = 0;
            foreach (string label in labels)
            {
                Debug.Log($"Starting download for label: {label}");
                InitProgressDisplay.SetCurrentAssetIndex(i);
                
                var handle = Addressables.LoadAssetsAsync<Object>(label, null);
                
                handles.Add(handle);
                
                while (!handle.IsDone)
                {
                    InitProgressDisplay.UpdateLoadingBar(handle.PercentComplete);
                    yield return null; // Wait until the next frame
                }
                
                // yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Assets for label {label} downloaded successfully.");
                    InitProgressDisplay.UpdateLoadingBar(1f);
                }
                else
                {
                    Debug.LogError($"Failed to download assets for label {label}.");
                    errorText.gameObject.SetActive(true);
                    errorText.text = "There was an error downloading bundle: " + label;
                }
                
                i++;
            }

            // Optionally, here you can use the assets as needed, now that they are downloaded.
            // For example, adding them to a pool, instantiating, etc.

            // Make sure to release the handles when done to avoid memory leaks.
            foreach (var handle in handles)
            {
                Addressables.Release(handle);
            }

            Debug.Log("All downloads completed.");
            SceneNavigator.GoToLogin();
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
}