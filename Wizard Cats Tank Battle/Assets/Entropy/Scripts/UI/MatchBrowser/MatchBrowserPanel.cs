using Fusion;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI.MatchBrowser
{
    public class MatchBrowserPanel : GamePanel
    {
        public override void OpenPanel()
        {
            base.OpenPanel();
            PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);
            
            NetworkRunner runner = FindAnyObjectByType<NetworkRunner>();

            if (!runner.IsRunning)
            {
                NetworkManagerCustom.GetInstance().Connect(NetworkMode.Online);
            }
        }
    }
}