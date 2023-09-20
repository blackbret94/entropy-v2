using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Vashta.Entropy.UI
{
    public class ScreenResolutionDropdown : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Dropdown resolutionDropdown;

        private Resolution[] _resolutions;
        private List<Resolution> _filteredResolutions;

        private double _currentRefreshRate;
        private int _currentResolutionIndex = 0;

        private void Start()
        {
            _filteredResolutions = new List<Resolution>();
            
            // clear old options
            resolutionDropdown.ClearOptions();
            _currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

            // iterate over supported resolutions
            _resolutions = Screen.resolutions;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                // if (Math.Abs(_resolutions[i].refreshRateRatio.value - _currentRefreshRate) < .01)
                // {
                    _filteredResolutions.Add(_resolutions[i]);
                // }
            }

            // stringify resolutions
            List<string> options = new List<string>();
            for (int i = 0; i < _filteredResolutions.Count; i++)
            {
                Resolution resolution = _filteredResolutions[i];
                int refreshRateInt = Mathf.RoundToInt((float)resolution.refreshRateRatio.value);
                string aspectRatio = GetAspectRatio(resolution.width, resolution.height);
                string resolutionOption = resolution.width + "x" + resolution.height + " " + refreshRateInt + "Hz " + aspectRatio + "";
                options.Add(resolutionOption);

                // identify the currently selected resolution
                if (resolution.width == Screen.width && resolution.height == Screen.height)
                {
                    _currentResolutionIndex = i;
                }
            }
            
            // Add remaining options
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = _currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, true);
        }

        private string GetAspectRatio(int x, int y)
        {
            return string.Format("{0}:{1}",x/GreatestCommonDivisor(x,y), y/GreatestCommonDivisor(x,y));
        }
        
        private int GreatestCommonDivisor(int a, int b)
        {
            while( b != 0 )
            {
                var remainder = a % b;
                a = b;
                b = remainder;
            }

            return a;
        }
    }
}