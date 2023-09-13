using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.ItemStore
{
    public class HatItemStoreCategory: AbstractItemStoreCategory
    {
        protected override void InitCategory()
        {
            Category = WardrobeCategory.HAT;
        }
        
        protected override void IndexItems()
        {
            _activeItemList = new List<ScriptableWardrobeItem>();
            
            // iterate over hats
            foreach (var hat in Wardrobe.Hats)
            {
                // add ones that aren't owned
                if (!hat.AvailAtStart && hat.IsForSale && !PlayerInventory.OwnsHatById(hat.Id))
                {
                    _activeItemList.Add(hat);
                }
            }
        }

        protected override void Purchase()
        {
            PlayerInventory.AddHat((Hat)ActiveItem());
        }
    }
}