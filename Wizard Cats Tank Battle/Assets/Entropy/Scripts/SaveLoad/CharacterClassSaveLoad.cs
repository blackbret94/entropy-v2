using System;
using CBS;
using ExitGames.Client.Photon;
using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.SaveLoad
{
    public class CharacterClassSaveLoad : MonoBehaviour
    {
        private IProfile ProfileModule { get; set; }
        private bool _hasInit;

        private Player _player;
        private const string key = "classId";
        
        private void Init()
        {
            if (_hasInit)
                return;
            
            ProfileModule = CBSModule.Get<CBSProfile>();
            _player = GetComponent<Player>();

            _hasInit = true;
        }
        
        public void Save(int classId)
        {
            Init();
            ProfileModule.SaveProfileData(key, classId.ToString(), OnSaveData);
        }
        
        private void OnSaveData(CBSSaveProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                // Debug.Log("Successfully updated class ID");
            }
        }

        public void Load()
        {
            Init();
            ProfileModule.GetProfileData(key, OnGetData);
        }
        
        private void OnGetData(CBSGetProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                Int32.TryParse(result.Data[key], out int classId);
                
                // update player
                _player.LoadClassCallback(classId, true);
            }
        }

        public static string DefaultClassByStringEncrypted()
        {
            return new CharacterClassSerializable().Encrypt();
        }

        public static void SetCurrentClassAsCustomProperty()
        {
            SetCurrentClassAsCustomProperty(PlayerPrefs.GetString(PrefsKeys.characterClass, DefaultClassByStringEncrypted()));
        }

        private static void SetCurrentClassAsCustomProperty(string encrypted)
        {
            Hashtable setCharacterClass = new Hashtable();
            setCharacterClass.Add(PrefsKeys.characterClass, encrypted);
            PhotonNetwork.LocalPlayer.SetCustomProperties(setCharacterClass);
        }
    }
}