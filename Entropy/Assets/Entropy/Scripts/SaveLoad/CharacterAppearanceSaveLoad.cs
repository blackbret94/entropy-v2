using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.SaveLoad
{
    public class CharacterAppearanceSaveLoad: MonoBehaviour
    {
        public const string PlayerPrefsKey = "CharacterAppearance";
        
        public void Save(CharacterAppearanceSerializable serializable)
        {
            PlayerPrefs.SetString(PlayerPrefsKey, serializable.ToJson());
        }

        public CharacterAppearanceSerializable Load()
        {
            string appearanceJson = PlayerPrefs.GetString(PlayerPrefsKey, new CharacterAppearanceSerializable().ToJson());
            return CharacterAppearanceSerializable.FromJson(appearanceJson);
        }
    }
}