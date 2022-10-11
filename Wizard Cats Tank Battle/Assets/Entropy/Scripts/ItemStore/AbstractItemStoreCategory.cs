using System.Collections.Generic;
using Entropy.Scripts.Audio;
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
        public SfxController SfxController;
        
        protected int _itemIndex = 0;
        protected List<ScriptableWardrobeItem> _activeItemList;
        protected CurrencyTransaction _currencyTransaction;
        
        private void Awake()
        {
            _currencyTransaction = new CurrencyTransaction();
            Init();
        }

        private void Init()
        {
            PlayerInventory.Init();
            IndexItems();
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

        public void SetItem()
        {
            foreach (Transform child in ObjectRoot.transform)
            {
                Destroy(child.gameObject);
            }

            
            if(_activeItemList.Count > 0)
                Instantiate(_activeItemList[_itemIndex].ItemObject, ObjectRoot.transform);
            else
                Debug.Log("Attempted to create an item in an empty category!");
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
                SfxController.PlayPurchase();

                Init();
                
                if (_itemIndex >= _activeItemList.Count)
                    _itemIndex = 0;
                
                SetItem();

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
            if(_itemIndex < _activeItemList.Count && _activeItemList.Count > 0)
                return _activeItemList[_itemIndex].Cost;

            return 0;
        }
    }
}