using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Character
{
    [Serializable]
    public class CharacterAppearanceSerializable
    {
        public string HatId;
        public string BodyId;
        public string SkinId;
        public string CartId;

        public CharacterAppearanceSerializable()
        {
            HatId = "1";
            BodyId = "1";
            SkinId = "1";
            CartId = "1";
        }
        
        public CharacterAppearanceSerializable(string hatId, string bodyId, string skinId, string cartId)
        {
            HatId = hatId;
            BodyId = bodyId;
            SkinId = skinId;
            CartId = cartId;
        }

        public string Encrypt()
        {
            return Encryptor.Encrypt(ToJson());
        }

        public static CharacterAppearanceSerializable Decrypt(string encrypted)
        {
            string decrypted = Encryptor.Decrypt(encrypted);
            return FromJson(decrypted);
        }
        
        private string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        private static CharacterAppearanceSerializable FromJson(string json)
        {
            return JsonUtility.FromJson<CharacterAppearanceSerializable>(json);
        }
    }
}