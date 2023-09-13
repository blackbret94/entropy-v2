using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.ItemStore
{
    public class TankItemStoreCategory: AbstractItemStoreCategory
    {
        protected override void InitCategory()
        {
            Category = WardrobeCategory.CART;
        }
        
        protected override void IndexItems()
        {
            _activeItemList = new List<ScriptableWardrobeItem>();
            
            // iterate over hats
            foreach (var cart in Wardrobe.Carts)
            {
                // add ones that aren't owned
                if (!cart.AvailAtStart && cart.IsForSale && !PlayerInventory.OwnsCartById(cart.Id))
                {
                    _activeItemList.Add(cart);
                }
            }
        }

        protected override void Purchase()
        {
            PlayerInventory.AddCart((Cart)ActiveItem());
        }
    }
}