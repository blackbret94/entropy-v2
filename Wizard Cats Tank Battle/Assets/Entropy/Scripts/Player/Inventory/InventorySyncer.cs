using System.Collections.Generic;
using CBS;
using UnityEngine;
using Vashta.Entropy.Character;

namespace Entropy.Scripts.Player.Inventory
{
    public class InventorySyncer : MonoBehaviour
    {
        public PlayerInventory PlayerInventory;
        public CharacterAppearance CharacterAppearance;
        
        private ICBSInventory InventoryModule { get; set; }
        private const string HAT_CATEGORY_NAME = "Hats";
        private const string TANK_CATEGORY_NAME = "Carts";
        private const string WAND_CATEGORY_NAME = "Wands";
        private const string BODY_CATEGORY_NAME = "Bodies";
        private const string SKIN_CATEGORY_NAME = "Skins";
        private const string MEOW_CATEGORY_NAME = "Meows";
            
        private void Start()
        {
            InventoryModule = CBSModule.Get<CBSInventory>();
            InventoryModule.OnItemAdded += OnItemAdded;
        }

        private void OnItemAdded(InventoryItemGrandResult result)
        {
            if (result.IsSuccess)
            {
                Debug.LogFormat("Item {0} added to inventory", result.InventoryItem.DisplayName);
            }
        }

        public void RefreshInventory()
        {
            InventoryModule.GetInventory(OnGetInventory);
        }
        
        private void OnGetInventory(GetInventoryResult result)
        {
            if (result.IsSuccess)
            {
                PlayerInventory.ClearPurchasable();
                RefreshInventory(result.AllItems);
                RefreshEquipment(result.EquippedItems);
            }
        }

        private void RefreshInventory(List<CBSInventoryItem> items)
        {
            foreach (var item in items)
            {
                string category = item.Category;
                string id = item.ID;
                if (category == HAT_CATEGORY_NAME)
                {
                    PlayerInventory.AddHatById(id);
                } else if (category == TANK_CATEGORY_NAME)
                {
                    PlayerInventory.AddCartById(id);
                } else if (category == WAND_CATEGORY_NAME)
                {
                    PlayerInventory.AddTurretById(id);
                }
            }
        }

        private void RefreshEquipment(List<CBSInventoryItem> items)
        {
            string hatId = "";
            string cartId = "";
            string turretId = "";
            string bodyId = "";
            string skinId = "";
            string meowId = "";
            
            foreach (var item in items)
            {
                string category = item.Category;
                string id = item.ID;
                if (category == HAT_CATEGORY_NAME)
                {
                    hatId = id;
                } else if (category == TANK_CATEGORY_NAME)
                {
                    cartId = id;
                } else if (category == WAND_CATEGORY_NAME)
                {
                    turretId = id;
                } else if (category == BODY_CATEGORY_NAME)
                {
                    bodyId = id;
                } else if (category == SKIN_CATEGORY_NAME)
                {
                    skinId = id;
                } else if (category == MEOW_CATEGORY_NAME)
                {
                    meowId = id;
                }
            }
            
            CharacterAppearance.SetOutfit(hatId, cartId, turretId, bodyId, skinId, meowId);
        }

        public void SaveAppearance(string hatId, string cartId, string wandId, string bodyId, string skinId, string meowId)
        {
            InventoryModule.EquipItem(hatId, OnEquip);
            InventoryModule.EquipItem(cartId, OnEquip);
            InventoryModule.EquipItem(wandId, OnEquip);
            InventoryModule.EquipItem(bodyId, OnEquip);
            InventoryModule.EquipItem(skinId, OnEquip);
        }
        
        private void OnEquip(EquipInventoryItemResult result)
        {
            if (result.IsSuccess)
            {
                Debug.LogFormat("Item {0} was equiped", result.InventoryItemId);
            }
        }
    }
}