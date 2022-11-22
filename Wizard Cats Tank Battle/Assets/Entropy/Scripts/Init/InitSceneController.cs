using System.Collections.Generic;
using Jacovone.AssetBundleMagic;
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
        
        private AssetBundleMagic.Progress p = null;
        private bool _isComplete = false;

        private void Start()
        {
            AssetBundleMagic.DownloadVersions (delegate(string versions) {
                Debug.Log ("Received versions:\n" + versions);
                
                LoadBundles();
            }, delegate(string error) {
                Debug.LogError (error);
            });
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
                SceneNavigator.GoToMainMenuAsync();
                _isComplete = true;
                return;
            }
            
            InitProgressDisplay.SetCurrentAssetIndex(i);
            string bundleName = BundlesToDownload[i];
                
            p = AssetBundleMagic.DownloadBundle (
                bundleName,
                delegate(AssetBundle ab) {
                    Debug.Log("Bundle finished downloading! " + bundleName);
                    LoadBundleByIndex(i+1, maxBundleIndex);

                },
                delegate(string error) {
                    // errorText.enabled = true;
                    // errorText.text = "Error: " + error;
                    Debug.LogError (error);
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

    }
}