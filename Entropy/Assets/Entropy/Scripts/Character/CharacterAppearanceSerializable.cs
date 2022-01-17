using System;
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

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static CharacterAppearanceSerializable FromJson(string json)
        {
            return JsonUtility.FromJson<CharacterAppearanceSerializable>(json);
        }
    }
}