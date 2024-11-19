using System;
using Entropy.Scripts.Player.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeItemInfoBox : GamePanel
    {
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DescriptionText;
        public TextMeshProUGUI ItemsCollectedText;
        public Image Image;
        public Image BackgroundImage;
        public GameObject PurchaseButton;
        public TextMeshProUGUI PurchaseButtonText;
        public PlayerInventory PlayerInventory;
        
        public PlayerCharacterWardrobe PlayerCharacterWardrobe;
        public RarityDictionary RarityDictionary;

        private ScriptableWardrobeItem _selectedItem;

        public void SetItem(ScriptableWardrobeItem selectedItem)
        {
            if (!PlayerCharacterWardrobe)
            {
                Debug.LogError("Missing connection to PlayerCharacterWardrobe");
                return;
            }
            
            _selectedItem = selectedItem;

            if (_selectedItem == null)
                return;

            Rarity rarity = _selectedItem.Rarity;
            RarityDefinition rarityDefinition = RarityDictionary[rarity];
            
            BackgroundImage.sprite = rarityDefinition.BackgroundImage;
            bool isOwned = _selectedItem.Category == WardrobeCategory.BODY_TYPE || _selectedItem.Category == WardrobeCategory.SKIN || PlayerInventory.OwnsItemById(_selectedItem.Id);
            
            TitleText.text = selectedItem.ItemName;
            DescriptionText.text = selectedItem.ItemDescription;
            ItemsCollectedText.text = GetCollectedText();
            Image.sprite = selectedItem.Icon;

            if (isOwned)
            {
                PurchaseButton.SetActive(false);
            }
            else
            {
                PurchaseButton.SetActive(true);
                PurchaseButtonText.text = "Buy " + selectedItem.Cost.ToString();
            }
            
        }

        public void RefreshPanel()
        {
            SetItem(_selectedItem);
        }

        private string GetCollectedText()
        {
            int numCollected = 0;
            int numTotal = 0;

            switch (_selectedItem.Category)
            {
                case WardrobeCategory.HAT:
                    numCollected = PlayerInventory.Hats.Count;
                    numTotal = PlayerCharacterWardrobe.Hats.Count;
                    break;
                case WardrobeCategory.BODY_TYPE:
                    numCollected = PlayerInventory.BodyTypes.Count;
                    numTotal = PlayerCharacterWardrobe.BodyTypes.Count;
                    break;
                case WardrobeCategory.SKIN:
                    numCollected = PlayerInventory.BodyTypes[0].SkinOptions.Count;
                    numTotal = PlayerCharacterWardrobe.BodyTypes[0].SkinOptions.Count;
                    break;
                case WardrobeCategory.CART:
                    numCollected = PlayerInventory.Carts.Count;
                    numTotal = PlayerCharacterWardrobe.Carts.Count;
                    break;
                case WardrobeCategory.MEOW:
                    numCollected = PlayerInventory.Meows.Count;
                    numTotal = PlayerCharacterWardrobe.Meows.Count;
                    break;
                case WardrobeCategory.TURRET:
                    numCollected = PlayerInventory.Turrets.Count;
                    numTotal = PlayerCharacterWardrobe.Turrets.Count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return $"Collected {numCollected} of {numTotal}";
        }
    }
}