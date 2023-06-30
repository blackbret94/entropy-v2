using System;
using System.Collections;
using System.Collections.Generic;
using Jacovone.AssetBundleMagic;
using TMPro;
using UnityEngine;
using Vashta.Entropy.SceneNavigation;
using Vashta.Entropy.UI;

namespace Vashta.Entropy.Scripts.Init
{
    public class InitSceneController : MonoBehaviour
    {
        public SceneNavigator SceneNavigator;
        public InitProgressDisplay InitProgressDisplay;
        public List<string> BundlesToDownload;
        public TextMeshProUGUI errorText;
        public GameObject ConnectionErrorPanel;
        
        private AssetBundleMagic.Progress p = null;
        private bool _isComplete = false;

        private string _testUrl = "www.google.com";

        private void Start()
        {
            errorText.gameObject.SetActive(false);
            
            AssetBundleMagic.DownloadVersions (delegate(string versions) {
                // Debug.Log ("Received versions:\n" + versions);
                
                LoadBundles();
            }, delegate(string error) {
                Debug.LogError (error);
                ConnectionErrorPanel.SetActive(true);
            });

            StartCoroutine(CheckInternetConnection());
        }
        
        public void ClearCache ()
        {
            AssetBundleMagic.CleanBundlesCache ();
        }

        private void LoadBundles()
        {
            Debug.Log("Loading bundles");

            int numberOfBundles = BundlesToDownload.Count;
            
            InitProgressDisplay.SetMaxIndex(numberOfBundles);
            LoadBundleByIndex(0, numberOfBundles);
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
                
            p = AssetBundleMagic.DownloadBundle (
                bundleName,
                delegate(AssetBundle ab) {
                    // Debug.Log("Bundle finished downloading! " + bundleName);
                    LoadBundleByIndex(i+1, maxBundleIndex);

                },
                delegate(string error) {
                    errorText.gameObject.SetActive(true);
                    errorText.enabled = true;
                    errorText.text = "Error: " + error;
                    Debug.LogError (error);
                    
                    // This may cause issues.  It is meant to prevent the app from failing to launch after an update,
                    // but there may be cases where it is best if it terminates here.
                    LoadBundleByIndex(i+1, maxBundleIndex);
                }
            );
        }
        
        private void Update ()
        {
            if (p != null) 
            {
                // Update loading bar
                float pr = p.GetProgress ();
                if (pr < 0)
                    pr = 0;
                
                InitProgressDisplay.UpdateLoadingBar(pr);
            }
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