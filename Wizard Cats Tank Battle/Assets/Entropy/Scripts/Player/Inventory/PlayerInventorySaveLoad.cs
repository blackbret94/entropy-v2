using System.Collections.Generic;
using CBS;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;
using Newtonsoft.Json;

namespace Entropy.Scripts.Player.Inventory
{
    public class PlayerInventorySaveLoad
    {
        private PlayerCharacterWardrobe _playerCharacterWardrobe;
        private PlayerInventory _playerInventory;

        private IProfile ProfileModule { get; set; }
        private const string
            HatKey = "inventoryHat",
            CartKey = "inventoryCart",
            BodyKey = "inventoryBody",
            TurretsKey = "inventoryTurret",
            MeowsKey = "inventoryMeow";

        private bool _hatIsLoaded,
            _cartIsLoaded,
            _bodyIsLoaded,
            _turretIsLoaded,
            _meowIsLoaded;

        public PlayerInventorySaveLoad(PlayerCharacterWardrobe playerCharacterWardrobe, PlayerInventory playerInventory)
        {
            _playerCharacterWardrobe = playerCharacterWardrobe;
            _playerInventory = playerInventory;
            ProfileModule = CBSModule.Get<CBSProfile>();
        }

        public bool IsLoaded()
        {
            return _hatIsLoaded && _cartIsLoaded && _bodyIsLoaded && _turretIsLoaded && _meowIsLoaded;
        }

        public void Load()
        {
            ProfileModule.GetProfileData(HatKey, OnGetLoadHats);
            ProfileModule.GetProfileData(CartKey, OnGetLoadCarts);
            ProfileModule.GetProfileData(BodyKey, OnGetLoadBodies);
            ProfileModule.GetProfileData(TurretsKey, OnGetLoadTurrets);
            ProfileModule.GetProfileData(MeowsKey, OnGetLoadMeows);
        }

        public void Save()
        {
            SaveHats(_playerInventory.Hats);
            SaveCart(_playerInventory.Carts);
            SaveTurret(_playerInventory.Turrets);
        }

        private List<string> DeserializeInventoryResults(CBSGetProfileDataResult result, string key)
        {
            List<string> ownedObjectIds = new List<string>();
                
            if(result.Data.ContainsKey(key))
            {
                string ids = result.Data[key];
                    
                ownedObjectIds =JsonConvert.DeserializeObject<List<string>>(ids);

                if (ownedObjectIds == null)
                    ownedObjectIds = new List<string>();
            }

            return ownedObjectIds;
        }
        
        private void OnGetLoadHats(CBSGetProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                List<string> ownedObjectIds = DeserializeInventoryResults(result, HatKey);
                
                List<Hat> ownedObjects = new List<Hat>();
                
                foreach (Hat item in _playerCharacterWardrobe.Hats)
                {
                    if ((item.AvailAtStart || ownedObjectIds.Contains(item.Id)))
                    {
                        ownedObjects.Add(item);
                    }
                }
                
                _playerInventory.Hats = ownedObjects;
                _hatIsLoaded = true;
            }
            else
            {
                Debug.LogError("Error loading hats: " + result.Error.Message);
            }
        }

        private void SaveHats(List<Hat> hats)
        {
            List<string> hatIds = new List<string>();
            foreach (var hat in hats)
            {
                hatIds.Add(hat.Id);
            }
            
            ProfileModule.SaveProfileData(HatKey, JsonConvert.SerializeObject(hatIds), OnSaveData);
        }
        
        private void OnSaveData(CBSSaveProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("User data saved successfully");
            }
            else
            {
                Debug.Log("Error saving user data: " + result.Error.Message);
            }
        }
        
        private void OnGetLoadBodies(CBSGetProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                List<string> ownedObjectIds = DeserializeInventoryResults(result, BodyKey);
                
                List<BodyType> ownedObjects = new List<BodyType>();
                
                foreach (BodyType item in _playerCharacterWardrobe.BodyTypes)
                {
                    if ((item.AvailAtStart || ownedObjectIds.Contains(item.Id)))
                    {
                        ownedObjects.Add(item);
                    }
                }
                
                _playerInventory.BodyTypes = ownedObjects;
                _bodyIsLoaded = true;
            }
            else
            {
                Debug.LogError("Error loading BodyTypes: " + result.Error.Message);
            }
        }

        private void OnGetLoadCarts(CBSGetProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                List<string> ownedObjectIds = DeserializeInventoryResults(result, CartKey);
                
                List<Cart> ownedObjects = new List<Cart>();
                
                foreach (Cart item in _playerCharacterWardrobe.Carts)
                {
                    if ((item.AvailAtStart || ownedObjectIds.Contains(item.Id)))
                    {
                        ownedObjects.Add(item);
                    }
                }
                
                _playerInventory.Carts = ownedObjects;
                _cartIsLoaded = true;
            }
            else
            {
                Debug.LogError("Error loading Carts: " + result.Error.Message);
            }
        }

        private void SaveCart(List<Cart> carts)
        {
            List<string> ids = new List<string>();
            foreach (var cart in carts)
            {
                ids.Add(cart.Id);
            }
            
            ProfileModule.SaveProfileData(CartKey, JsonConvert.SerializeObject(ids), OnSaveData);
        }

        private void OnGetLoadTurrets(CBSGetProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                List<string> ownedObjectIds = DeserializeInventoryResults(result, TurretsKey);
                
                List<Turret> ownedObjects = new List<Turret>();
                
                foreach (Turret item in _playerCharacterWardrobe.Turrets)
                {
                    if ((item.AvailAtStart || ownedObjectIds.Contains(item.Id)))
                    {
                        ownedObjects.Add(item);
                    }
                }
                
                _playerInventory.Turrets = ownedObjects;
                _turretIsLoaded = true;
            }
            else
            {
                Debug.LogError("Error loading turrets: " + result.Error.Message);
            }
        }
        
        private void SaveTurret(List<Turret> turrets)
        {
            List<string> ids = new List<string>();
            foreach (var turret in turrets)
            {
                ids.Add(turret.Id);
            }
            
            ProfileModule.SaveProfileData(TurretsKey, JsonConvert.SerializeObject(ids), OnSaveData);
        }
        
        private void OnGetLoadMeows(CBSGetProfileDataResult result)
        {
            if (result.IsSuccess)
            {
                List<string> ownedObjectIds = DeserializeInventoryResults(result, MeowsKey);
                
                List<Meow> ownedObjects = new List<Meow>();
                
                foreach (Meow item in _playerCharacterWardrobe.Meows)
                {
                    if ((item.AvailAtStart || ownedObjectIds.Contains(item.Id)))
                    {
                        ownedObjects.Add(item);
                    }
                }
                
                _playerInventory.Meows = ownedObjects;
                _meowIsLoaded = true;
            }
            else
            {
                Debug.LogError("Error loading meows: " + result.Error.Message);
            }
        }
    }
}