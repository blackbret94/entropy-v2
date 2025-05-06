using Entropy.Scripts.Audio;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.IO;
using Vashta.Entropy.PostProcessing;
using Vashta.Entropy.SaveLoad;
using PrefsKeys = TanksMP.PrefsKeys;

namespace Vashta.Entropy.UI
{
    public class SettingsPanel : GamePanel
    {
        public bool isOnMainMenu;
        
        public Slider volumeSlider;
        public Slider musicSlider;
        public Slider graphicsSlider;
        public Slider brightnessSlider;

        public Toggle musicToggle;
        public Toggle leftHandedModeToggle;
        public Toggle showMinimapToggle;
        public Toggle showFlashingLightsToggle;
        
        public JoystickPlacementController joystickPlacementController;

        public MusicController MusicController;

        public Toggle FullscreenToggle;
        public Toggle AimArrowToggle;
        
        public string WebsiteUrl = "https://wizardcatstankbattle.com";
        public string PrivacyPolicyUrl = "https://vashtaentertainment.com/privacy_policy.html";
        
        private CurrentSettings _currentSettings;
        
        private void Awake()
        {
            _currentSettings = CurrentSettings.Instance();
        }
        
        public override void OpenPanel()
        {
            base.OpenPanel();
            ReadSettings();
            
            if(!isOnMainMenu)
                HUDPanel.Get().ClosePanel();
        }
        
        public override void ClosePanel()
        {
            base.ClosePanel();
            ApplySettings();
            
            if(!isOnMainMenu)
                HUDPanel.Get().OpenPanel();
        }

        private void ReadSettings()
        {
            _currentSettings.Load();
            
            musicToggle.isOn = SettingsReader.GetMusicIsOn();
            volumeSlider.value = SettingsReader.GetVolume();
            musicSlider.value = SettingsReader.GetMusicVolume();
            
            leftHandedModeToggle.isOn = SettingsReader.GetLeftHandedMode();
            AimArrowToggle.isOn = SettingsReader.GetAimArrow();
            showMinimapToggle.isOn = SettingsReader.GetShowMinimap();
            showFlashingLightsToggle.isOn = SettingsReader.GetShowFlashingLights();
            brightnessSlider.value = SettingsReader.GetBrightness();
            
            if(FullscreenToggle)
                FullscreenToggle.isOn = Screen.fullScreen;
        }

        public void ApplySettings()
        {
            _currentSettings.MusicIsOn = musicToggle.isOn;
            _currentSettings.Volume = volumeSlider.value;
            _currentSettings.MusicVolume = musicSlider.value;
            _currentSettings.LeftHandedMode = leftHandedModeToggle.isOn;
            _currentSettings.AimArrowOn = AimArrowToggle.isOn;
            _currentSettings.ShowMinimap = showMinimapToggle.isOn;
            _currentSettings.ShowFlashingLights = showFlashingLightsToggle.isOn;
            _currentSettings.Brightness = brightnessSlider.value;
            
            if (graphicsSlider)
            {
                int qualityLevel = Mathf.RoundToInt(graphicsSlider.value);
                QualitySettings.SetQualityLevel(qualityLevel);
                _currentSettings.GraphicsQuality = Mathf.RoundToInt(graphicsSlider.value);
            }
   
            _currentSettings.Save();
            
            if (!isOnMainMenu)
            {
                UIGame.GetInstance().RefreshAimArrow();
                UIGame.GetInstance().Minimap.SetActive(showMinimapToggle.isOn);
            }
        }

        public void OnLeftHandedModeChanged(bool leftHandedModeEnabled)
        {
            joystickPlacementController.ApplyChanges(leftHandedModeEnabled);
        }
        
        public void OnMusicChanged(bool value)
        {
            MusicController.AudioSource.enabled = musicToggle.isOn;
            MusicController.PlayMusic();
        }

        public void OnToggleMinimap(bool value)
        {
            if(!isOnMainMenu)
                UIGame.GetInstance().Minimap.SetActive(value);
        }

        public void OnAimArrowChanged(bool value)
        {
            if (!isOnMainMenu)
            {
                UIGame.GetInstance().RefreshAimArrow();
            }
        }

        public void SetFullscreen(bool b)
        {
            Screen.fullScreen = b;
        }
        
        public void OnVolumeChanged(float value)
        {
            volumeSlider.value = value;
            AudioListener.volume = value;
        }

        public void OnMusicVolumeChanged(float value)
        {
            musicSlider.value = value;
            MusicController.AudioSource.volume = value;
        }
        
        public void OpenWebsite()
        {
            Application.OpenURL(WebsiteUrl);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(PrivacyPolicyUrl);
        }

        public void OnBrightnessChanged(float value)
        {
            // CameraPostProcessingController[] controllers = FindObjectsByType<CameraPostProcessingController>(FindObjectsSortMode.None);
            //
            // foreach (CameraPostProcessingController controller in controllers)
            // {
            //     controller.SetBrightness(value);
            // }
        }
    }
}