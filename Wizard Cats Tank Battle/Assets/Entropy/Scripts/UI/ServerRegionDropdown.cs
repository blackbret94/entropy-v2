using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class ServerRegionDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown RegionsDropdown;

        public NetworkManagerCustom NetworkManagerCustom;
        public List<ServerRegion> Regions;

        private int _currentIndex;

        private void Start()
        {
            List<string> options = new List<string>();

            for (int i = 0; i < Regions.Count; i++)
            {
                options.Add(Regions[i].Stringify());
            }
            
            LoadRegion();
            
            RegionsDropdown.ClearOptions();
            RegionsDropdown.AddOptions(options);
            RegionsDropdown.value = _currentIndex;
        }

        private void LoadRegion()
        {
            if (NetworkManagerCustom.RegionController == null)
            {
                Debug.LogWarning("Could not load region!");
                return;
            }

            string regionToken = NetworkManagerCustom.RegionController.Region;

            for (int i=0; i<Regions.Count; i++)
            {
                ServerRegion region = Regions[i];

                if (region.Token == regionToken)
                {
                    _currentIndex = i;
                    return;
                }
            }
        }

        public void SetRegion(int regionIndex)
        {
            ServerRegion region = Regions[regionIndex];
            
            NetworkManagerCustom.RegionController.Region = region.Token;

            // Reconnect
            if (UIMain.GetInstance().Runner.IsRunning)
            {
                NetworkManagerCustom.GetInstance().Reconnect();
            }
        }
    }
}