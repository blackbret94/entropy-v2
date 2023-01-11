using CBS;
using CBS.Playfab;
using UnityEngine;

namespace Vashta.Entropy.Scripts.CBSIntegration
{
    public class ItemsIntegration : MonoBehaviour
    {
        private ICBSItems ItemModule { get; set; }

        private void Start()
        {
            ItemModule = CBSModule.Get<CBSItems>();

            ItemModule.OnItemPurchased += OnItemPurchased;
            ItemModule.OnItemGranted += OnItemGranted;
        }
        
        private void OnItemPurchased(CBSPurchaseItemResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("User has successfully bought an item with id " + result.PurchasedItem.ID);
            }
        }
        
        private void OnItemGranted(GrantItemResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("The user has been successfully granted with an item from the ID " + result.GrantItem.ID);
            }
        }

        public void PurchaseItem(string itemId, string currencyCode, int cost)
        {
            ItemModule.PurchaseItem(itemId, currencyCode, cost, OnPurchaseItem);
        }
        
        private void OnPurchaseItem(CBSPurchaseItemResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("Item ID = " + result.PurchasedItem.ID);
            }
        }

        public void GrantItem(string itemId)
        {
            ItemModule.GrantItem(itemId, OnGrantItem);
        }
        
        private void OnGrantItem(GrantItemResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("Item ID = " + result.GrantItem.ID);
            }
        }

        public void GetCategories()
        {
            ItemModule.GetCategories(ItemType.ITEMS, OnGetItemsCategories);
        }
        
        private void OnGetItemsCategories(GetCategoriesResult result)
        {
            if (result.IsSuccess)
            {
                foreach(var category in result.Categories)
                {
                    Debug.Log(category);
                }
            }
        }

        public void GetItemsByCategory(string category)
        {
            ItemModule.GetItemsByCategory(category, OnGetItems);
        }
        
        private void OnGetItems(GetItemsResult result)
        {
            if (result.IsSuccess)
            {
                foreach (var item in result.Items)
                {
                    Debug.Log("Item ID = "+item.ID);
                }
            }
        }

        public void GetItem(string itemId)
        {
            ItemModule.GetItemByID(itemId, OnGetItem);
        }
        
        private void OnGetItem(GetItemResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("Item ID = " + result.Item.ID);
            }
        }
    }
}