using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI.MatchBrowser
{
    public class MatchBrowserPanel : GamePanel
    {
        public override void OpenPanel()
        {
            base.OpenPanel();

            if (!PhotonNetwork.IsConnected)
            {
                PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);
                NetworkManagerCustom.GetInstance().Connect(NetworkMode.Online);
            }
        }
    }
}