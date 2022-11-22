using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class InitProgressDisplay: MonoBehaviour
    {
        public Slider LoadingBarSlider;
        public TextMeshProUGUI LoadingBarText;
        public string LoadingBarTextPreface = "";
        
        public Transform RotatingCircle;
        public float RotatingCircleSpeed = .5f;

        private int _currentAssetIndex = 1;
        private int _maxAssetIndex;

        public void SetCurrentAssetIndex(int i)
        {
            _currentAssetIndex = i + 1;
        }

        public void SetMaxIndex(int i)
        {
            _maxAssetIndex = i;
        }

        private void Update()
        {
            RotatingCircle.Rotate(0f, 0f, RotatingCircleSpeed * Time.deltaTime);
        }

        public void UpdateLoadingBar(float perunComplete)
        {
            LoadingBarSlider.value = perunComplete*100;
            LoadingBarText.text = LoadingBarTextPreface + FormatPercent(perunComplete);
        }

        private string FormatPercent(float perun)
        {
            return $"{Mathf.Floor(perun * 100)}% (Downloading {_currentAssetIndex}/{_maxAssetIndex})";
        }
    }
}