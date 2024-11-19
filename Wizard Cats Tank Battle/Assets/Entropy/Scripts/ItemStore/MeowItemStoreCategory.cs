using System.Collections.Generic;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.ItemStore
{
    public class MeowItemStoreCategory : AbstractItemStoreCategory
    {
        protected override void InitCategory()
        {
            Category = WardrobeCategory.MEOW;
        }

        protected override void IndexItems()
        {
            _activeItemList = new List<ScriptableWardrobeItem>();
            
            // iterate over hats
            foreach (var meow in Wardrobe.Meows)
            {
                // add ones that aren't owned
                if (!meow.AvailAtStart && meow.IsForSale && !PlayerInventory.OwnsMeowById(meow.Id))
                {
                    _activeItemList.Add(meow);
                }
            }
        }

        protected override void Purchase()
        {
            PlayerInventory.AddMeow((Meow)ActiveItem());
        }
    }
}