using UnityEngine;
using Vashta.Entropy.IO;

namespace Vashta.Entropy.SaveLoad
{
    public class CurrentSettings
    {
        public bool MusicIsOn;
        public float Volume;
        public float MusicVolume;
        public bool LeftHandedMode;
        public bool AimArrowOn;
        public bool ShowMinimap;
        public int GraphicsQuality;
        public bool ShowFlashingLights;
        public float Brightness;

        private static CurrentSettings _instance;
        
        public static CurrentSettings Instance()
        {
            if (_instance == null)
            {
                _instance = new CurrentSettings();
                _instance.Load();
            }
            
            return _instance;
        }

        public void Load()
        {
            MusicIsOn = SettingsReader.GetMusicIsOn();
            Volume = SettingsReader.GetVolume();
            MusicVolume = SettingsReader.GetMusicVolume();
            LeftHandedMode = SettingsReader.GetLeftHandedMode();
            AimArrowOn = SettingsReader.GetAimArrow();
            ShowMinimap = SettingsReader.GetShowMinimap();
            ShowFlashingLights = SettingsReader.GetShowFlashingLights();
            GraphicsQuality = SettingsReader.GetGraphicsSettings();
            Brightness = SettingsReader.GetBrightness();
        }

        public void Save()
        {
            PlayerPrefs.SetInt(PrefsKeys.playMusic, MusicIsOn ? 1 : 0);
            PlayerPrefs.SetFloat(PrefsKeys.appVolume, Volume);
            PlayerPrefs.SetFloat(PrefsKeys.musicVolume, MusicVolume);
            PlayerPrefs.SetInt(PrefsKeys.lefthandedMode, LeftHandedMode ? 1 : 0);
            PlayerPrefs.SetInt(PrefsKeys.aimArrow, AimArrowOn ? 1 : 0);
            PlayerPrefs.SetInt(PrefsKeys.showMinimap, ShowMinimap ? 1 : 0);
            PlayerPrefs.SetInt(PrefsKeys.flashingLights, ShowFlashingLights ? 1 : 0);
            PlayerPrefs.SetInt(PrefsKeys.graphicsSettings, GraphicsQuality);
            PlayerPrefs.SetFloat(PrefsKeys.brightness, Brightness);
            PlayerPrefs.Save();
        }
    }
}