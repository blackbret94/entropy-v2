using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Vashta.Entropy.SaveLoad;

namespace Vashta.Entropy.PostProcessing
{
    public class CameraPostProcessingController : MonoBehaviour
    {
        private PostProcessVolume _postProcessVolume;
        private AutoExposure _exposure;
        private float _minBrightness = -2;
        private float _maxBrightness = 2;
        
        private void Awake()
        {
            _postProcessVolume = GetComponent<PostProcessVolume>();
            _postProcessVolume.profile.TryGetSettings(out _exposure);
        }

        private void Start()
        {
            CurrentSettings.Instance().Load();
            SetBrightness(CurrentSettings.Instance().Brightness);
        }

        public void SetBrightness(float value)
        {
            float brightness = Mathf.Lerp(_minBrightness, _maxBrightness, value);
            
            if (_exposure != null)
            {
                _exposure.keyValue.value = brightness;
                Debug.Log("Setting brightness to: " + brightness);
            }
        }
    }
}