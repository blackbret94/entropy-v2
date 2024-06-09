using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.IO
{
    public class SettingsReader : MonoBehaviour
    {
        public static bool GetMusicIsOn()
        {
            return bool.Parse(PlayerPrefs.GetString(PrefsKeys.playMusic));
        }

        public static float GetVolume()
        {
            return PlayerPrefs.GetFloat(PrefsKeys.appVolume);
        }

        public static bool GetLeftHandedMode()
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.lefthandedMode, 0));;
        }

        public static bool GetAimArrow()
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.aimArrow, 0));
        }
    }
}