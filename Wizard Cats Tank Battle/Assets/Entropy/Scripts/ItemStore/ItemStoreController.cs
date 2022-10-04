using TMPro;
using UnityEngine;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.ItemStore
{
    public class ItemStoreController: MonoBehaviour
    {
        private AbstractItemStoreCategory _activeCategory;

        public TextMeshProUGUI PurchaseText;
        public HatItemStoreCategory
            HatItemStoreCategory;

        public PlayerGoldPanel PlayerGoldPanel;

        private void Start()
        {
            Init();
        }
        
        public void Init()
        {
            SetCategoryHats();
        }

        public void SetCategoryHats()
        {
            _activeCategory = HatItemStoreCategory;
            SetPriceText();
        }

        public void NextItem()
        {
            _activeCategory.NextItem();
            SetPriceText();
        }

        public void PreviousItem()
        {
            _activeCategory.PreviousItem();
            SetPriceText();
        }

        public void AttemptPurchaseItem()
        {
            if (_activeCategory.AttemptPurchaseItem())
            {
                PlayerGoldPanel.Refresh();
                SetPriceText();
            }
        }

        private void SetPriceText()
        {
            PurchaseText.text = "Buy " + _activeCategory.GetCostOfActiveItem();
        }
    }
}