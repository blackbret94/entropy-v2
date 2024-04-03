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
        
        private bool _isComplete = false;

        private string _testUrl = "www.google.com";

        private void Start()
        {
            errorText.gameObject.SetActive(false);
            StartCoroutine(DownloadAssetsForLabels(BundlesToDownload));
            
            // AddressableManager.DownloadVersions (delegate(string versions) {
                // Debug.Log ("Received versions:\n" + versions);
            //     
            //     LoadBundles();
            // }, delegate(string error) {
            //     Debug.LogError (error);
            //     ConnectionErrorPanel.SetActive(true);
            // });

            StartCoroutine(CheckInternetConnection());
        }
        
        // public void ClearCache ()
        // {
        //     AddressableManager.CleanBundlesCache ();
        // }

        private void LoadBundles()
        {
            Debug.Log("Loading bundles");

            int numberOfBundles = BundlesToDownload.Count;
            
            InitProgressDisplay.SetMaxIndex(numberOfBundles);
            UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<Object>(BundlesToDownload, OnAssetLoaded).Completed +=
                OnAllAssetsLoaded;
            // LoadBundleByIndex(0, numberOfBundles);
        }
        
        private IEnumerator DownloadAssetsForLabels(List<string> labels)
        {
            List<AsyncOperationHandle> handles = new List<AsyncOperationHandle>();

            foreach (string label in labels)
            {
                Debug.Log($"Starting download for label: {label}");
                var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<Object>(label, null);
                handles.Add(handle);
                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Assets for label {label} downloaded successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to download assets for label {label}.");
                }
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
            _isComplete = true;
        }

        private void LoadBundleByIndex(int i, int maxBundleIndex)
        {
            if (i >= maxBundleIndex)
            {
                // go to next scene
                // SceneNavigator.GoToMainMenuAsync();
                SceneNavigator.GoToLogin();
                _isComplete = true;
                return;
            }
            
            InitProgressDisplay.SetCurrentAssetIndex(i);
            string bundleName = BundlesToDownload[i];

            Addressables.LoadAssetsAsync<Object>(bundleName, OnAssetLoaded).Completed +=
                OnAllAssetsLoaded;
            // {
            //     Debug.Log("Bundle finished downloading! " + bundleName);
            //     LoadBundleByIndex(i+1, maxBundleIndex);
            // });

            // p = AddressableManager.DownloadBundle (
            //     bundleName,
            //     delegate(AssetBundle ab) {
            //         // Debug.Log("Bundle finished downloading! " + bundleName);
            //         LoadBundleByIndex(i+1, maxBundleIndex);
            //
            //     },
            //     delegate(string error) {
            //         errorText.gameObject.SetActive(true);
            //         errorText.enabled = true;
            //         errorText.text = "Error: " + error;
            //         Debug.LogError (error);
            //         
            //         // This may cause issues.  It is meant to prevent the app from failing to launch after an update,
            //         // but there may be cases where it is best if it terminates here.
            //         LoadBundleByIndex(i+1, maxBundleIndex);
            //     }
            // );
        }
        
        private void OnAssetLoaded(Object loadedAsset)
        {
            // This callback is called for each asset that is loaded.
            // You can instantiate the asset, or add it to a list for later use, etc.
            Debug.Log($"Loaded asset: {loadedAsset.name}");
        }

        private void OnAllAssetsLoaded(AsyncOperationHandle<IList<Object>> obj)
        {
            // This callback is called once all assets with the specified label have been loaded.
            Debug.Log("All assets loaded.");
            SceneNavigator.GoToLogin();
            _isComplete = true;
            // Here, you could do additional setup with the loaded assets, such as initializing game levels, settings, etc.
        }
        
        private void Update ()
        {
            // if (p != null) 
            // {
            //     // Update loading bar
            //     float pr = p.GetProgress ();
            //     if (pr < 0)
            //         pr = 0;
            //     
            //     InitProgressDisplay.UpdateLoadingBar(pr);
            // }
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