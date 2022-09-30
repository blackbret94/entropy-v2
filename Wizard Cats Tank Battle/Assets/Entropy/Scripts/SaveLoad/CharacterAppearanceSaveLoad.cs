using Photon.Pun;
using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.SaveLoad
{
    public class CharacterAppearanceSaveLoad: MonoBehaviour
    {
        public void Save(CharacterAppearanceSerializable appearanceSerializable)
        {
            string encrypted = appearanceSerializable.Encrypt();
            Debug.Log("Saved string: " + encrypted);
            
            PlayerPrefs.SetString(PrefsKeys.characterAppearance, encrypted);
            SetAppearanceAsCustomProperty(encrypted);
        }

        public CharacterAppearanceSerializable Load()
        {
            string appearance = PlayerPrefs.GetString(PrefsKeys.characterAppearance, DefaultAppearanceStringEncrypted());
            Debug.Log("Appearance: " + appearance);
            return CharacterAppearanceSerializable.Decrypt(appearance);
        }

        public static string DefaultAppearanceStringEncrypted()
        {
            return new CharacterAppearanceSerializable().Encrypt();
        }


        public static void SetCurrentAppearanceAsCustomProperty()
        {
            SetAppearanceAsCustomProperty(PlayerPrefs.GetString(PrefsKeys.characterAppearance, DefaultAppearanceStringEncrypted()));
        }
        
        private static void SetAppearanceAsCustomProperty(string encrypted)
        {
            ExitGames.Client.Photon.Hashtable setPlayerAppearance = new ExitGames.Client.Photon.Hashtable();
            setPlayerAppearance.Add(PrefsKeys.characterAppearance, encrypted);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setPlayerAppearance); 
        }
    }
}