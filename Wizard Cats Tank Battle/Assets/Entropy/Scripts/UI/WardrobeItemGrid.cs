using System;
using System.Collections.Generic;
using Entropy.Scripts.Player.Inventory;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeItemGrid : MonoBehaviour
    {
        public Transform ItemScrollboxContentRoot;
        public GameObject ItemBoxPrefab;
        
        public PlayerCharacterWardrobe UniverseWardrobe;
        public PlayerInventory PlayerInventory;
        public CharacterAppearance CharacterAppearance;
        public WardrobeOptionsPanel WardrobeOptionsPanel;
        private WardrobeCategory _activeCategory;
        private List<WardrobeItemUI> _itemBoxes;

        private void Start()
        {
            _itemBoxes = new List<WardrobeItemUI>();
            PlayerInventory.Init();
            SetCategoryHat();
        }

        public void Refresh()
        {
            SetCategory(_activeCategory);
        }

        public void SetCategoryHat()
        {
            SetCategory(WardrobeCategory.HAT);
        }

        public void SetCategoryBodyType()
        {
            SetCategory(WardrobeCategory.BODY_TYPE);
        }

        public void SetCategorySkin()
        {
            SetCategory(WardrobeCategory.SKIN);
        }

        public void SetCategoryCart()
        {
            SetCategory(WardrobeCategory.CART);
        }

        public void SetCategoryMeow()
        {
            SetCategory(WardrobeCategory.MEOW);
        }

        public void SetCategoryTurret()
        {
            SetCategory(WardrobeCategory.TURRET);
        }
        
        public void SetCategory(WardrobeCategory wardrobeCategory)
        {
            if (!UniverseWardrobe)
            {
                Debug.LogError("Missing player character wardrobe!");
                return;
            }
            
            Clear();
            
            _activeCategory = wardrobeCategory;
            
            // Get items
            List<ScriptableWardrobeItem> items;
            string selectedItemId;
            
            switch (wardrobeCategory) 
            {
                case WardrobeCategory.HAT:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Hats);
                    selectedItemId = CharacterAppearance.Hat.Id;
                    break;
                case WardrobeCategory.BODY_TYPE:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.BodyTypes);
                    selectedItemId = CharacterAppearance.Body.Id;
                    break;
                case WardrobeCategory.SKIN:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.GetRandomBodyType().SkinOptions);
                    selectedItemId = CharacterAppearance.Skin.Id;
                    break;
                case WardrobeCategory.CART:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Carts);
                    selectedItemId = CharacterAppearance.Cart.Id;
                    break;
                case WardrobeCategory.MEOW:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Meows);
                    selectedItemId = CharacterAppearance.Meow.Id;
                    break;
                case WardrobeCategory.TURRET:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Turrets);
                    selectedItemId = CharacterAppearance.Turret.Id;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wardrobeCategory), wardrobeCategory, null);
            }
            
            // Inflate
            foreach (ScriptableWardrobeItem wardrobeItem in items)
            {
                GameObject wardrobeItemBox = Instantiate(ItemBoxPrefab, ItemScrollboxContentRoot);
                WardrobeItemUI wardrobeItemUI = wardrobeItemBox.GetComponent<WardrobeItemUI>();

                if (!wardrobeItemUI)
                {
                    Debug.LogError("Item box is missing WardrobeItemUI component!");
                    continue;
                }

                bool isOwned = false;
                if (wardrobeCategory == WardrobeCategory.BODY_TYPE || wardrobeCategory == WardrobeCategory.SKIN)
                {
                    isOwned = true;
                }
                else
                {
                    isOwned = PlayerInventory.OwnsItemById(wardrobeItem.Id);
                }
                
                wardrobeItemUI.Inflate(wardrobeItem, isOwned, WardrobeOptionsPanel);
                _itemBoxes.Add(wardrobeItemUI);
                
                // check if this is the selected item
                if (wardrobeItem.Id == selectedItemId)
                {
                    WardrobeOptionsPanel.SelectItem(wardrobeItem);
                    wardrobeItemUI.Select();
                }
            }
        }

        public void DeselectAllItemBoxes()
        {
            foreach (WardrobeItemUI itemBox in _itemBoxes)
            {
                itemBox.Deselect();
            }
        }

        public WardrobeCategory GetActiveCategory()
        {
            return _activeCategory;
        }

        private void Clear()
        {
            _itemBoxes.Clear();
            
            foreach (Transform child in ItemScrollboxContentRoot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}