using System.Collections.Generic;
using Entropy.Scripts.Audio;
using Entropy.Scripts.Currency;
using Entropy.Scripts.Player.Inventory;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI;

namespace Entropy.Scripts.ItemStore
{
    public abstract class AbstractItemStoreCategory: MonoBehaviour
    {
        public PlayerCharacterWardrobe Wardrobe;
        public PlayerInventory PlayerInventory;
        public GameObject ObjectRoot;
        public SfxController SfxController;

        private Collider _objectCollider;
        
        protected int _itemIndex = 0;
        protected List<ScriptableWardrobeItem> _activeItemList;
        
        public WardrobeCategory Category = WardrobeCategory.HAT;
        
        private void Awake()
        {
            _objectCollider = ObjectRoot.GetComponentInParent<Collider>();
            Init();
        }

        protected abstract void InitCategory();

        private void Init()
        {
            InitCategory();
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
            _objectCollider.enabled = false;
        }

        public void Show()
        {
            _objectCollider.enabled = true;
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

            if (CurrencyTransaction.Instance.QueryPurchase(cost))
            {
                Debug.Log("Purchasing " + ActiveItem().Id);
                
                // buy
                CurrencyTransaction.Instance.DecreaseCurrency(cost);
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