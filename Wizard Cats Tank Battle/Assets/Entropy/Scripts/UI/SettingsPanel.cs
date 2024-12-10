using Entropy.Scripts.Audio;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.IO;

namespace Vashta.Entropy.UI
{
    public class SettingsPanel : GamePanel
    {
        public bool isOnMainMenu;
        
        public Slider volumeSlider;
        public Slider musicSlider;
        public Slider graphicsSlider;

        public Toggle musicToggle;
        public Toggle leftHandedModeToggle;
        public Toggle showMinimapToggle;
        
        public JoystickPlacementController joystickPlacementController;

        public MusicController MusicController;

        public Toggle FullscreenToggle;
        public Toggle AimArrowToggle;
        
        public string WebsiteUrl = "https://wizardcatstankbattle.com";
        public string PrivacyPolicyUrl = "https://vashtaentertainment.com/privacy_policy.html";
        
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
            musicToggle.isOn = SettingsReader.GetMusicIsOn();
            
            volumeSlider.value = SettingsReader.GetVolume();
            musicSlider.value = SettingsReader.GetMusicVolume();
            
            leftHandedModeToggle.isOn = SettingsReader.GetLeftHandedMode();
            AimArrowToggle.isOn = SettingsReader.GetAimArrow();
            showMinimapToggle.isOn = SettingsReader.GetShowMinimap();
            
            if(FullscreenToggle)
                FullscreenToggle.isOn = Screen.fullScreen;
        }

        public void ApplySettings()
        {
            PlayerPrefs.SetString(PrefsKeys.playMusic, musicToggle.isOn.ToString());
            PlayerPrefs.SetFloat(PrefsKeys.appVolume, volumeSlider.value);
            PlayerPrefs.SetFloat(PrefsKeys.musicVolume, musicSlider.value);
            PlayerPrefs.SetInt(PrefsKeys.lefthandedMode, leftHandedModeToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt(PrefsKeys.aimArrow, AimArrowToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt(PrefsKeys.showMinimap, showMinimapToggle.isOn ? 1 : 0);

            if (!isOnMainMenu)
            {
                UIGame.GetInstance().RefreshAimArrow();
                UIGame.GetInstance().Minimap.SetActive(showMinimapToggle.isOn);
            }

            if (graphicsSlider)
            {
                int qualityLevel = Mathf.RoundToInt(graphicsSlider.value);
                QualitySettings.SetQualityLevel(qualityLevel);
                PlayerPrefs.SetInt(PrefsKeys.graphicsSettings, qualityLevel);
            }

            PlayerPrefs.Save();
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
    }
}