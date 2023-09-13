using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.ItemStore
{
    public class WandItemStoreCategory: AbstractItemStoreCategory
    {
        protected override void InitCategory()
        {
            Category = WardrobeCategory.TURRET;
        }
        
        protected override void IndexItems()
        {
            _activeItemList = new List<ScriptableWardrobeItem>();
            
            // iterate over hats
            foreach (var turret in Wardrobe.Turrets)
            {
                // add ones that aren't owned
                if (!turret.AvailAtStart && turret.IsForSale && !PlayerInventory.OwnsTurretById(turret.Id))
                {
                    _activeItemList.Add(turret);
                }
            }
        }

        protected override void Purchase()
        {
            PlayerInventory.AddTurret((Turret)ActiveItem());
        }
    }
}