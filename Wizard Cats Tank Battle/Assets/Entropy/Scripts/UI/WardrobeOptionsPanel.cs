using System;
using Entropy.Scripts.Audio;
using Entropy.Scripts.Currency;
using Entropy.Scripts.Player.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeOptionsPanel: GamePanel
    {
        [FormerlySerializedAs("WardrobeItemBox")] public WardrobeItemGrid wardrobeItemGrid;
        [FormerlySerializedAs("WardrobeItemText")] public WardrobeItemInfoBox wardrobeItemInfoBox;
        
        private ScriptableWardrobeItem _selectedItem;
        public CharacterAppearance CharacterAppearance;
        public PlayerInventory PlayerInventory;
        public SfxController SfxController;
        
        public void SelectItem(ScriptableWardrobeItem scriptableWardrobeItem)
        {
            _selectedItem = scriptableWardrobeItem;
            wardrobeItemInfoBox.SetItem(scriptableWardrobeItem);
            wardrobeItemGrid.DeselectAllItemBoxes();

            string id = scriptableWardrobeItem.Id;
            Debug.Log("Category: " + scriptableWardrobeItem.Category.ToString());

            switch (scriptableWardrobeItem.Category)
            {
                case WardrobeCategory.HAT:
                    CharacterAppearance.SetHat(id);
                    break;
                case WardrobeCategory.BODY_TYPE:
                    CharacterAppearance.SetBodyType(id);
                    break;
                case WardrobeCategory.SKIN:
                    CharacterAppearance.SetSkin(id);
                    break;
                case WardrobeCategory.CART:
                    CharacterAppearance.SetCart(id);
                    break;
                case WardrobeCategory.MEOW:
                    CharacterAppearance.SetMeow(id);
                    break;
                case WardrobeCategory.TURRET:
                    CharacterAppearance.SetTurret(id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Refresh()
        {
            CharacterAppearance.RefreshAppearance();
        }

        public void AttemptToPurchaseSelectedItem()
        {
            int cost = _selectedItem.Cost;

            if (CurrencyTransaction.Instance.QueryPurchase(cost))
            {
                Debug.Log("Purchasing " + _selectedItem.Id);
                
                // buy
                CurrencyTransaction.Instance.DecreaseCurrency(cost);
                Purchase();
                SfxController.PlayPurchase();
                
                wardrobeItemInfoBox.RefreshPanel();
                wardrobeItemGrid.Refresh();
            }
            else
            {
                Debug.Log("Not purchasing" + _selectedItem.Id);
                SfxController.PlayNoCoins();
            }
        }

        private void Purchase()
        {
            WardrobeCategory category = _selectedItem.Category;

            switch (category)
            {
                case WardrobeCategory.HAT:
                    PlayerInventory.AddHat((Hat)_selectedItem);
                    break;
                case WardrobeCategory.BODY_TYPE:
                    // PlayerInventory.AddHat((Hat)_selectedItem);
                    break;
                case WardrobeCategory.SKIN:
                    // PlayerInventory.AddSki((Hat)_selectedItem);
                    break;
                case WardrobeCategory.CART:
                    PlayerInventory.AddCart((Cart)_selectedItem);
                    break;
                case WardrobeCategory.MEOW:
                    PlayerInventory.AddMeow((Meow)_selectedItem);
                    break;
                case WardrobeCategory.TURRET:
                    PlayerInventory.AddTurret((Turret)_selectedItem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}