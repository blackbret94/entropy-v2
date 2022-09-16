using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "WardrobeData", menuName = "Entropy/PlayerCharacterWardrobe", order = 1)]
    public class PlayerCharacterWardrobe : UnityEngine.ScriptableObject
    {
        public List<Hat> Hats;
        public List<BodyType> BodyTypes;
        public List<Cart> Carts;
        public List<Turret> Turrets;

        public Hat GetRandomHat()
        {
            return Hats[Random.Range(0, Hats.Count)];
        }

        public Hat GetHatById(int id)
        {
            int index = GetIndexFromId(id);
            
            if (index >= Hats.Count)
                return Hats[0];

            return Hats[index];
        }

        public BodyType GetRandomBodyType()
        {
            return BodyTypes[Random.Range(0, BodyTypes.Count)];
        }

        public BodyType GetBodyTypeById(int id)
        {
            int index = GetIndexFromId(id);
            
            if (index >= BodyTypes.Count)
                return BodyTypes[0];

            return BodyTypes[index];
        }

        public Skin GetSkinById(int bodyTypeId, int skinId)
        {
            int skinIndex = GetIndexFromId(skinId);

            BodyType body = GetBodyTypeById(bodyTypeId);

            if (skinIndex >= body.SkinOptions.Count)
                return body.SkinOptions[0];

            return body.SkinOptions[skinIndex];
        }

        public Cart GetRandomCart()
        {
            return Carts[Random.Range(0, Carts.Count)];
        }

        public Cart GetCartById(int id)
        {
            int index = GetIndexFromId(id);
            
            if (index >= Carts.Count)
                return Carts[0];

            return Carts[index];
        }

        public Turret GetRandomTurret()
        {
            return Turrets[Random.Range(0, Turrets.Count)];
        }

        private int GetIndexFromId(int id)
        {
            return id - 1;
        }

        private int GetIdFromIndex(int index)
        {
            return index + 1;
        }
    }
}
