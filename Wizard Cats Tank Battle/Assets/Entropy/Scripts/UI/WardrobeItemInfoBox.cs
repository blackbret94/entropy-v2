using Entropy.Scripts.Player.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeItemInfoBox : GamePanel
    {
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DescriptionText;
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
            bool isOwned = PlayerInventory.OwnsItemById(_selectedItem.Id);
            
            TitleText.text = selectedItem.ItemName;
            DescriptionText.text = selectedItem.ItemDescription;
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
    }
}