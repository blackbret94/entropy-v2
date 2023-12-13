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
            PlayerPrefs.SetInt(PrefsKeys.networkMode, (int)NetworkMode.Online);

            if (!PhotonNetwork.IsConnected)
            {
                NetworkManagerCustom.GetInstance().Connect(NetworkMode.Online);
            }
        }
    }
}