using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.ItemStore
{
    public class WandItemStoreCategory: AbstractItemStoreCategory
    {
        protected override void IndexItems()
        {
            _activeItemList = new List<ScriptableWardrobeItem>();
            
            // iterate over hats
            foreach (var turret in Wardrobe.Turrets)
            {
                // add ones that aren't owned
                if (!turret.AvailAtStart && !PlayerInventory.OwnsTurretById(turret.Id))
                {
                    Debug.Log("Adding turret with ID: " + turret.Id);
                    _activeItemList.Add(turret);
                }
                else
                {
                    Debug.Log("Turret is owned with ID: " + turret.Id);
                }
            }
        }

        protected override void Purchase()
        {
            PlayerInventory.AddTurret((Turret)ActiveItem());
        }
    }
}