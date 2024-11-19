using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeItemText : GamePanel
    {
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DescriptionText;
        public Image Image;
        public GameObject PurchaseButton;
        public TextMeshProUGUI PurchaseButtonText;
        public PlayerCharacterWardrobe PlayerCharacterWardrobe;

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

            bool isOwned = PlayerCharacterWardrobe.ContainsId(_selectedItem.Id);
            
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
                PurchaseButtonText.text = selectedItem.Cost.ToString();
            }
            
        }

        public void RefreshPanel()
        {
            SetItem(_selectedItem);
        }
    }
}