using System.Collections.Generic;
using Entropy.Scripts.Currency;
using Entropy.Scripts.Player.Inventory;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.ItemStore
{
    public abstract class AbstractItemStoreCategory: MonoBehaviour
    {
        public PlayerCharacterWardrobe Wardrobe;
        public PlayerInventory PlayerInventory;
        public GameObject ObjectRoot;

        protected int _itemIndex = 0;
        protected List<ScriptableWardrobeItem> _activeItemList;
        protected CurrencyTransaction _currencyTransaction;
        
        private void Start()
        {
            _currencyTransaction = new CurrencyTransaction();
            Init();
        }

        private void Init()
        {
            IndexItems();
            SetItem();
        }

        protected ScriptableWardrobeItem ActiveItem()
        {
            return _activeItemList[_itemIndex];
        }
        
        protected abstract void IndexItems();

        public void Hide()
        {
            ObjectRoot.SetActive(false);
        }

        public void Show()
        {
            ObjectRoot.SetActive(true);
        }
        
        public void NextItem()
        {
            _itemIndex++;
            if (_itemIndex >= _activeItemList.Count)
                _itemIndex = 0;
            
            SetItem();
        }

        public void PreviousItem()
        {
            _itemIndex--;
            if (_itemIndex < 0)
                _itemIndex = _activeItemList.Count - 1;

            SetItem();
        }

        private void SetItem()
        {
            foreach (Transform child in ObjectRoot.transform)
            {
                Destroy(child.gameObject);
            }

            Instantiate(_activeItemList[_itemIndex].ItemObject, ObjectRoot.transform);
        }
        
        public bool AttemptPurchaseItem()
        {
            int cost = ActiveItem().Cost;

            if (_currencyTransaction.QueryPurchase(cost))
            {
                Debug.Log("Purchasing " + ActiveItem().Id);
                
                // buy
                _currencyTransaction.ModifyCurrency(-cost);
                Purchase();

                if (_itemIndex >= _activeItemList.Count)
                    _itemIndex = 0;
                
                Init();
                return true;
            }
            else
            {
                Debug.Log("Not purchasing" + ActiveItem().Id);
                // don't buy    
                return false;
            }
        }

        protected abstract void Purchase();

        public int GetCostOfActiveItem()
        {
            return _activeItemList[_itemIndex].Cost;
        }
    }
}