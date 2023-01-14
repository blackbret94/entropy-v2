using System.Collections.Generic;
using CBS;
using Photon.Pun;
using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.SaveLoad
{
    public class CharacterAppearanceSaveLoad: MonoBehaviour
    {
        public CharacterAppearance CharacterAppearance;
        
        private IProfile ProfileModule { get; set; }
        private const string CHAR_APPEARANCE_KEY = "active-outfit";

        private void Start()
        {
            ProfileModule = CBSModule.Get<CBSProfile>();
        }
        
        public void Save(CharacterAppearanceSerializable appearanceSerializable)
        {
            string encrypted = appearanceSerializable.Encrypt();
            Debug.Log("Saved string: " + encrypted);
            
            PlayerPrefs.SetString(PrefsKeys.characterAppearance, encrypted);
            ProfileModule.SaveProfileData(CHAR_APPEARANCE_KEY, encrypted, OnSaveData);
            SetAppearanceAsCustomProperty(encrypted);
        }
        
        private void OnSaveData(CBSSaveProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("User data saved successfully");
            }
            else
            {
                Debug.Log("Error saving user data: " + result.Error.Message);
            }
        }

        public void Load()
        {
            if(ProfileModule == null)
                ProfileModule = CBSModule.Get<CBSProfile>();
            
            ProfileModule.GetProfileData(CHAR_APPEARANCE_KEY, OnGetLoadData);
        }
        
        private void OnGetLoadData(CBSGetProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                string data;

                try
                {
                    data = result.Data[CHAR_APPEARANCE_KEY];
                }
                catch (KeyNotFoundException e)
                {
                    data = DefaultAppearanceStringEncrypted();
                }
                
                Debug.Log("User outfit data = "+ data);
                if (string.IsNullOrEmpty(data))
                {
                    data = DefaultAppearanceStringEncrypted();
                }

                CharacterAppearanceSerializable appearance = CharacterAppearanceSerializable.Decrypt(data);
                CharacterAppearance.LoadAppearanceCallback(appearance);
            }
            else
            {
                Debug.LogError("Error getting character appearance: " + result.Error.Message);
            }
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