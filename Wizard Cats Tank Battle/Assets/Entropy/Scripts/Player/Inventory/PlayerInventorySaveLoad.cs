using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;
using Newtonsoft.Json;

namespace Entropy.Scripts.Player.Inventory
{
    public class PlayerInventorySaveLoad
    {
        private PlayerCharacterWardrobe _playerCharacterWardrobe;

        private const string
            HatKey = "inventoryHats",
            CartKey = "inventoryCart",
            BodyKey = "inventoryBody",
            TurretsKey = "inventoryTurrets";

        public PlayerInventorySaveLoad(PlayerCharacterWardrobe playerCharacterWardrobe)
        {
            _playerCharacterWardrobe = playerCharacterWardrobe;
        }

        public void Load(PlayerInventory playerInventory)
        {
            playerInventory.Hats = LoadHats();
            playerInventory.Carts = LoadCart();
            playerInventory.BodyTypes = LoadBody();
            // playerInventory.Turrets = LoadTurret();
        }

        public void Save(PlayerInventory playerInventory)
        {
            SaveHats(playerInventory.Hats);
            // SaveBody(playerInventory.BodyTypes);
            // SaveCart(playerInventory.Carts);
            // SaveTurret(playerInventory.Turrets);
        }

        private List<string> GetOwnedObjectsList(string key)
        {
            string ownedObjectIdsString = PlayerPrefs.GetString(key);
            List<string> ownedObjectIds =JsonConvert.DeserializeObject<List<string>>(ownedObjectIdsString);

            if (ownedObjectIds == null)
                ownedObjectIds = new List<string>();

            return ownedObjectIds;
        }
        
        private List<Hat> LoadHats()
        {
            List<Hat> ownedObjects = new List<Hat>();
            
            // read from playerprefs
            List<string> ownedObjectIds = GetOwnedObjectsList(HatKey);

            // iterate over options, adding ones that are owned or start free
            foreach (Hat item in _playerCharacterWardrobe.Hats)
            {
                if ((item.AvailAtStart || ownedObjectIds.Contains(item.Id)))
                {
                    ownedObjects.Add(item);
                }
            }
            
            return ownedObjects;
        }

        private void SaveHats(List<Hat> hats)
        {
            List<string> hatIds = new List<string>();
            foreach (var hat in hats)
            {
                hatIds.Add(hat.Id);
            }
            
            PlayerPrefs.SetString(HatKey, JsonConvert.SerializeObject(hatIds));
        }

        private List<BodyType> LoadBody()
        {
            List<BodyType> ownedObjects = new List<BodyType>();
            
            // read from playerprefs
            List<string> ownedObjectIds = GetOwnedObjectsList(BodyKey);
            
            // iterate over options, adding ones that are owned or start free
            foreach (BodyType item in _playerCharacterWardrobe.BodyTypes)
            {
                if (item.AvailAtStart || ownedObjectIds.Contains(item.Id))
                    ownedObjects.Add(item);
            }
            
            return ownedObjects;
        }

        private void SaveBody(List<BodyType> bodies)
        {
            
        }

        private List<Cart> LoadCart()
        {
            List<Cart> ownedObjects = new List<Cart>();
            
            // read from playerprefs
            List<string> ownedObjectIds = GetOwnedObjectsList(CartKey);
            
            // iterate over options, adding ones that are owned or start free
            foreach (Cart item in _playerCharacterWardrobe.Carts)
            {
                if (item.AvailAtStart || ownedObjectIds.Contains(item.Id))
                    ownedObjects.Add(item);
            }
            
            return ownedObjects;
        }

        private void SaveCart(List<Cart> carts)
        {
            
        }

        private List<Turret> LoadTurret()
        {
            List<Turret> ownedObjects = new List<Turret>();
            
            // read from playerprefs
            List<string> ownedObjectIds = GetOwnedObjectsList(TurretsKey);
            
            // iterate over options, adding ones that are owned or start free
            foreach (Turret item in _playerCharacterWardrobe.Turrets)
            {
                if (item.AvailAtStart || ownedObjectIds.Contains(item.Id))
                    ownedObjects.Add(item);
            }
            
            return ownedObjects;
        }

        private void SaveTurret(List<Turret> turrets)
        {
            
        }
    }
}