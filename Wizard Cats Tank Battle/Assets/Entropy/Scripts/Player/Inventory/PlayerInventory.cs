using System;
using UnityEngine;
using System.Collections.Generic;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;
using Random = UnityEngine.Random;

namespace Entropy.Scripts.Player.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        public List<Hat> Hats;
        public List<BodyType> BodyTypes;
        public List<Cart> Carts;
        public List<Turret> Turrets;
        public List<Meow> Meows;
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
            Load();
            
            _hasInit = true;
        }

        private void Load()
        {
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

        public void AddHat(Hat hat){
            Hats.Add(hat);
            Save();
        }

        public bool OwnsHatById(string id)
        {
            foreach (var hat in Hats)
            {
                if (hat.Id == id)
                    return true;
            }

            return false;
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
        
        public bool OwnsCartById(string id)
        {
            foreach (var hat in Carts)
            {
                if (hat.Id == id)
                    return true;
            }

            return false;
        }
        
        public void AddCart(Cart cart){
            Carts.Add(cart);
            Save();
        }

        public Turret GetRandomTurret()
        {
            return Turrets[Random.Range(0, Turrets.Count)];
        }

        public Turret GetTurretByIndex(int index)
        {
            if (index >= Turrets.Count)
                return Turrets[0];

            return Turrets[index];
        }

        public int GetTurretIndexById(string id)
        {
            for (int i = 0; i < Turrets.Count; i++)
            {
                Turret turret = Turrets[i];
                if (turret.Id == id)
                    return i;
            }
            
            Debug.Log("Could not find turret with ID: " + id);
            return 0;
        }

        public bool OwnsTurretById(string id)
        {
            foreach (var turret in Turrets)
            {
                if (turret.Id == id)
                    return true;
            }

            return false;
        }
        
        public void AddTurret(Turret turret){
            Turrets.Add(turret);
            Save();
        }
        
        public Meow GetRandomMeow()
        {
            return Meows[Random.Range(0, Meows.Count)];
        }

        public Meow GetMeowByIndex(int index)
        {
            if (index >= Meows.Count)
                return Meows[0];

            return Meows[index];
        }

        public int GetMeowIndexById(string id)
        {
            for (int i = 0; i < Meows.Count; i++)
            {
                Meow meow = Meows[i];
                if (meow.Id == id)
                    return i;
            }
            
            Debug.Log("Could not find meow with ID: " + id);
            return 0;
        }

        public int GetItemCountByCategory(WardrobeCategory category)
        {
            switch (category)
            {
                case WardrobeCategory.HAT:
                    return Hats.Count;
                case WardrobeCategory.BODY_TYPE:
                    return BodyTypes.Count;
                case WardrobeCategory.SKIN:
                    return BodyTypes[0].SkinOptions.Count;
                case WardrobeCategory.CART:
                    return Carts.Count;
                case WardrobeCategory.MEOW:
                    return Meows.Count;
                case WardrobeCategory.TURRET:
                    return Turrets.Count;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}