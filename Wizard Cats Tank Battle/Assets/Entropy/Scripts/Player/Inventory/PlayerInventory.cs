using UnityEngine;
using System.Collections.Generic;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.Player.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        public List<Hat> Hats;
        public List<BodyType> BodyTypes;
        public List<Cart> Carts;
        public List<Turret> Turrets;
        public PlayerCharacterWardrobe PlayerCharacterWardrobe;

        private PlayerInventorySaveLoad _playerInventorySaveLoad;

        private bool _hasInit;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            if (_hasInit)
                return;

            _playerInventorySaveLoad = new PlayerInventorySaveLoad(PlayerCharacterWardrobe);

            _hasInit = true;
        }

        public void Load()
        {
            Init();
            _playerInventorySaveLoad.Load(this);
        }

        public void Save()
        {
            Init();
            _playerInventorySaveLoad.Save(this);
        }

        public Hat GetRandomHat()
        {
            return Hats[Random.Range(0, Hats.Count)];
        }

        public int GetHatIndexById(string id)
        {
            for (int i = 0; i < Hats.Count; i++)
            {
                Hat hat = Hats[i];
                if (hat.Id == id)
                    return i;
            }

            Debug.Log("Could not find hat with ID: " + id);
            return 0;
        }

        public Hat GetHatByIndex(int index)
        {
            if (index >= Hats.Count)
                return Hats[0];

            return Hats[index];
        }

        public BodyType GetRandomBodyType()
        {
            return BodyTypes[Random.Range(0, BodyTypes.Count)];
        }
        
        public int GetBodyTypeIndexById(string id)
        {
            for (int i = 0; i < BodyTypes.Count; i++)
            {
                BodyType bt = BodyTypes[i];
                if (bt.Id == id)
                    return i;
            }

            Debug.Log("Could not find BodyType with ID: " + id);
            return 0;
        }

        public BodyType GetBodyTypeByIndex(int index)
        {
            if (index >= BodyTypes.Count)
                return BodyTypes[0];

            return BodyTypes[index];
        }

        public Skin GetSkinByIndex(int bodyTypeIndex, int skinIndex)
        {
            BodyType body = GetBodyTypeByIndex(bodyTypeIndex);

            if (skinIndex >= body.SkinOptions.Count)
                return body.SkinOptions[0];

            return body.SkinOptions[skinIndex];
        }

        public int GetSkinIndexById(int bodyIndex, string id)
        {
            BodyType bodyType = BodyTypes[bodyIndex];
            int skinCount = bodyType.SkinOptions.Count;

            for (int i = 0; i < skinCount; i++)
            {
                Skin skin = bodyType.SkinOptions[i];
                if (skin.Id == id)
                    return i;
            }
            
            Debug.Log("Could not find Skin with ID: " + id);
            return 0;
        }
        
        public int GetCartIndexById(string id)
        {
            for (int i = 0; i < Carts.Count; i++)
            {
                Cart cart = Carts[i];
                if (cart.Id == id)
                    return i;
            }

            Debug.Log("Could not find cart with ID: " + id);
            return 0;
        }

        public Cart GetRandomCart()
        {
            return Carts[Random.Range(0, Carts.Count)];
        }

        public Cart GetCartByIndex(int index)
        {
            if (index >= Carts.Count)
                return Carts[0];

            return Carts[index];
        }

        public Turret GetRandomTurret()
        {
            return Turrets[Random.Range(0, Turrets.Count)];
        }
    }
}