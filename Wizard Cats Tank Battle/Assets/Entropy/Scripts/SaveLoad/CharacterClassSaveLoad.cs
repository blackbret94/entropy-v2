using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.SaveLoad
{
    public class CharacterClassSaveLoad : MonoBehaviour
    {
        public void Save(CharacterClassSerializable characterClassSerializable)
        {
            string encrypted = characterClassSerializable.Encrypt();
            
            PlayerPrefs.SetString(PrefsKeys.characterClass, encrypted);
            SetCurrentClassAsCustomProperty(encrypted);;
        }

        public CharacterClassSerializable Load()
        {
            string characterClass = PlayerPrefs.GetString(PrefsKeys.characterClass, DefaultClassByStringEncrypted());
            return CharacterClassSerializable.Decrypt(characterClass);
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