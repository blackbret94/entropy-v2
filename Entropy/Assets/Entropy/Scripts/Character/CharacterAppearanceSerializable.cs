using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Character
{
    [Serializable]
    public class CharacterAppearanceSerializable
    {
        public int HatId;
        public int BodyId;
        public int SkinId;
        public int CartId;

        public CharacterAppearanceSerializable()
        {
            HatId = 1;
            BodyId = 1;
            SkinId = 1;
            CartId = 1;
        }
        
        public CharacterAppearanceSerializable(int hatId, int bodyId, int skinId, int cartId)
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