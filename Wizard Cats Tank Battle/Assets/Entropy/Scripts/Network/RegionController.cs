using UnityEngine;

namespace Vashta.Entropy.Network
{
    public class RegionController
    {
        public string DefaultRegion = "us";
        private string _region;
        private const string REGION_PREFS_KEY = "wctb_regionIndex";

        public string Region
        {
            get => _region;
            set
            {
                PlayerPrefs.SetString(REGION_PREFS_KEY, value);
                _region = value;
            }
        }

        public RegionController()
        {
            Region = PlayerPrefs.GetString(REGION_PREFS_KEY, DefaultRegion);
        }
    }
}