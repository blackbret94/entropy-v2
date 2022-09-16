using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Character
{
    [Serializable]
    public class CharacterClassSerializable
    {
        public int ClassId;

        public CharacterClassSerializable()
        {
            ClassId = 1;
        }

        public CharacterClassSerializable(int classId)
        {
            ClassId = classId;
        }
        
        public string Encrypt()
        {
            return Encryptor.Encrypt(ToJson());
        }

        public static CharacterClassSerializable Decrypt(string encrypted)
        {
            string decrypted = Encryptor.Decrypt(encrypted);
            return FromJson(decrypted);
        }
        
        private string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
        private static CharacterClassSerializable FromJson(string json)
        {
            return JsonUtility.FromJson<CharacterClassSerializable>(json);
        }
    }
}