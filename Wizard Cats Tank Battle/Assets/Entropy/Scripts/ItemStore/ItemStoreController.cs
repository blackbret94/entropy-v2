using TMPro;
using UnityEngine;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.ItemStore
{
    public class ItemStoreController: MonoBehaviour
    {
        private AbstractItemStoreCategory _activeCategory;

        public TextMeshProUGUI PurchaseText;
        public HatItemStoreCategory HatItemStoreCategory;
        public TankItemStoreCategory TankItemStoreCategory;
        public WandItemStoreCategory WandItemStoreCategory;
        public PlayerGoldPanel PlayerGoldPanel;
        public ItemCollectedCounterPanel ItemCollectedCounterPanel;

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
            SetCategory(HatItemStoreCategory);
        }

        public void SetCategoryTanks()
        {
            SetCategory(TankItemStoreCategory);
        }

        public void SetCategoryTurrets()
        {
            SetCategory(WandItemStoreCategory);
        }

        private void SetCategory(AbstractItemStoreCategory category)
        {
            if(_activeCategory)
                _activeCategory.Hide();
            
            _activeCategory = category;
            _activeCategory.Show();
            _activeCategory.SetItem();
            ItemCollectedCounterPanel.UpdatePanel(category.Category);
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
            int cost = _activeCategory.GetCostOfActiveItem();

            if (cost > 0)
                PurchaseText.text = "Buy " + cost;
            else
                PurchaseText.text = "Purchased";
        }
    }
}