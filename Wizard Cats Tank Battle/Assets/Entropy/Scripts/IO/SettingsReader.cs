using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.IO
{
    public class SettingsReader : MonoBehaviour
    {
        public static bool GetMusicIsOn()
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.playMusic, 1));
        }

        public static float GetVolume()
        {
            return PlayerPrefs.GetFloat(PrefsKeys.appVolume);
        }
        
        public static float GetMusicVolume()
        {
            return PlayerPrefs.GetFloat(PrefsKeys.musicVolume, 1f);
        }

        public static bool GetLeftHandedMode()
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.lefthandedMode, 0));;
        }

        public static bool GetAimArrow()
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.aimArrow, 0));
        }

        public static bool GetShowMinimap()
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.showMinimap, 1));
        }

        public static int GetGraphicsSettings()
        {
            return PlayerPrefs.GetInt(PrefsKeys.graphicsSettings, QualitySettings.GetQualityLevel());
        }
        
        public static bool GetShowFlashingLights()
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.flashingLights, 1));
        }
    }
}