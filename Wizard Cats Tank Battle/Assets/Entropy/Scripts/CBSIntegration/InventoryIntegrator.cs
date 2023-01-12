using CBS;
using UnityEngine;

namespace Vashta.Entropy.Scripts.CBSIntegration
{
    // This class was written before I understood the API, and now better serves as a model in a lot of places.  Lots of
    // functionality should be removed/moved to different classes.
    public class InventoryIntegrator : MonoBehaviour
    {
        private ICBSInventory InventoryModule { get; set; }
        
        private void Start()
        {
            InventoryModule = CBSModule.Get<CBSInventory>();

            InventoryModule.OnItemEquiped += OnItemEquiped;
            InventoryModule.OnItemAdded += OnItemAdded;
        }

        private void OnItemEquiped(EquipInventoryItemResult result)
        {
            if (result.IsSuccess)
            {
                Debug.LogFormat("Inventory item {0} equiped", result.InventoryItemId);
            }
        }
        
        private void OnItemAdded(InventoryItemGrandResult result)
        {
            if (result.IsSuccess)
            {
                Debug.LogFormat("Item {0} added to inventory", result.InventoryItem.DisplayName);
            }
        }

        public void GetInventory()
        {
            InventoryModule.GetInventory(OnGetInventory);
        }
        
        private void OnGetInventory(GetInventoryResult result)
        {
            if (result.IsSuccess)
            {
                var allItems = result.AllItems;
                var equipedItems = result.EquippedItems;
                var notEquipedItems = result.NonEquippedItems;
                var equipableItems = result.EquippableItems;
            }
        }

        public GetInventoryResult GetCachedInventory()
        {
            return InventoryModule.GetInventoryFromCache();
        }

        public void GetInventoryByCategory(string itemCategory)
        {
            InventoryModule.GetInventoryByCategory(itemCategory, OnGetInventory);
        }
        
        public void EquiptItem(string itemId)
        {
            InventoryModule.EquipItem(itemId, OnEquip);
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