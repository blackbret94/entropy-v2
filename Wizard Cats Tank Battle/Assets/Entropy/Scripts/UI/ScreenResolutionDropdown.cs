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

        private int _currentResolutionIndex = 0;

        private void Start()
        {
            // clear old options
            resolutionDropdown.ClearOptions();

            // iterate over supported resolutions
            _resolutions = Screen.resolutions;

            // stringify resolutions
            List<string> options = new List<string>();
            for (int i = 0; i < _resolutions.Length; i++)
            {
                Resolution resolution = _resolutions[i];
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
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
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