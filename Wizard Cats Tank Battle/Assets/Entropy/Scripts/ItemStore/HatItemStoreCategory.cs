using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.ItemStore
{
    public class HatItemStoreCategory: AbstractItemStoreCategory
    {
        protected override void IndexItems()
        {
            _activeItemList = new List<ScriptableWardrobeItem>();
            
            // iterate over hats
            foreach (var hat in Wardrobe.Hats)
            {
                // add ones that aren't owned
                if (!hat.AvailAtStart && !PlayerInventory.OwnsHatById(hat.Id))
                {
                    Debug.Log("Adding hat with ID: " + hat.Id);
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