using System;
using System.Collections;
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

        public bool ShouldLoadInventory;

        public PlayerInventorySaveLoad PlayerInventorySaveLoad { get; private set; }

        private bool _hasInit;

        private void Start()
        {
            Init();
        }

        public void Init(bool forceRefresh = false)
        {
            if (_hasInit && !forceRefresh)
                return;

            if (ShouldLoadInventory)
            {
                PlayerInventorySaveLoad = new PlayerInventorySaveLoad(PlayerCharacterWardrobe, this);
                Load();
            }

            _hasInit = true;
        }

        public void ForceRefresh()
        {
            Init(true);
        }

        private void Load()
        {
            PlayerInventorySaveLoad.Load();
        }

        public void Save()
        {
            Init();
            PlayerInventorySaveLoad.Save();
        }

        public Hat GetRandomHat()
        {
            return Hats[Random.Range(0, Hats.Count)];
        }

        public void AddHat(Hat hat){
            Hats.Add(hat);
            Save();
        }

        public void AddHatById(string id)
        {
            Hat hat = PlayerCharacterWardrobe.GetHatById(id);
            if (hat != null)
            {
                AddHat(hat);
            }
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

            // Debug.Log("Could not find hat with ID: " + id);
            return 0;
        }

        public Hat GetHatById(string id)
        {
            return GetHatByIndex(GetHatIndexById(id));
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

            return 0;
        }

        public BodyType GetBodyTypeByIndex(int index)
        {
            if (index >= BodyTypes.Count)
                return BodyTypes[0];

            return BodyTypes[index];
        }

        public BodyType GetBodyTypeById(string id)
        {
            return GetBodyTypeByIndex(GetBodyTypeIndexById(id));
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
            
            // Debug.Log("Could not find Skin with ID: " + id);
            return 0;
        }

        public Skin GetSkinById(int bodyIndex, string id)
        {
            return GetSkinByIndex(bodyIndex, GetSkinIndexById(bodyIndex, id));
        }
        
        public int GetCartIndexById(string id)
        {
            for (int i = 0; i < Carts.Count; i++)
            {
                Cart cart = Carts[i];
                if (cart.Id == id)
                    return i;
            }

            // Debug.Log("Could not find cart with ID: " + id);
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

        public Cart GetCartById(string id)
        {
            return GetCartByIndex(GetCartIndexById(id));
        }
        
        public void AddCart(Cart cart){
            Carts.Add(cart);
            Save();
        }
        
        public void AddCartById(string id)
        {
            Cart cart = PlayerCharacterWardrobe.GetCartById(id);
            if (cart != null)
            {
                AddCart(cart);
            }
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
            
            // Debug.Log("Could not find turret with ID: " + id);
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

        public Turret GetTurretById(string id)
        {
            return GetTurretByIndex(GetTurretIndexById(id));
        }
        
        public void AddTurret(Turret turret){
            Turrets.Add(turret);
            Save();
        }
        
        public void AddTurretById(string id)
        {
            Turret turret = PlayerCharacterWardrobe.GetTurretById(id);
            if (turret != null)
            {
                AddTurret(turret);
            }
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
            
            // Debug.Log("Could not find meow with ID: " + id);
            return 0;
        }
        
        public bool OwnsMeowById(string id)
        {
            foreach (var meow in Meows)
            {
                if (meow.Id == id)
                    return true;
            }

            return false;
        }

        public Meow GetMeowById(string id)
        {
            return GetMeowByIndex(GetMeowIndexById(id));
        }
        
        public void AddMeow(Meow meow){
            Meows.Add(meow);
            Save();
        }
        
        public void AddMeowById(string id)
        {
            Meow meow = PlayerCharacterWardrobe.GetMeowById(id);
            if (meow != null)
            {
                AddMeow(meow);
            }
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

        public bool OwnsItemById(string id)
        {
            if (OwnsHatById(id))
                return true;

            if (OwnsCartById(id))
                return true;

            if (OwnsMeowById(id))
                return true;

            if (OwnsTurretById(id))
                return true;
            
            return false;
        }
    }
}