using System;
using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeItemBox : MonoBehaviour
    {
        public Transform ItemScrollboxContentRoot;
        public GameObject ItemBoxPrefab;
        
        public PlayerCharacterWardrobe UniverseWardrobe;
        public PlayerCharacterWardrobe PlayerCharacterWardrobe;
        public WardrobeOptionsPanel WardrobeOptionsPanel;
        private WardrobeCategory _activeCategory;

        private void Start()
        {
            SetCategoryHat();
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
            List<ScriptableWardrobeItem> items = new List<ScriptableWardrobeItem>();
            
            switch (wardrobeCategory) 
            {
                case WardrobeCategory.HAT:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Hats);
                    break;
                case WardrobeCategory.BODY_TYPE:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.BodyTypes);
                    break;
                case WardrobeCategory.SKIN:
                    items = new List<ScriptableWardrobeItem>(PlayerCharacterWardrobe.GetRandomBodyType().SkinOptions);
                    break;
                case WardrobeCategory.CART:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Carts);
                    break;
                case WardrobeCategory.MEOW:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Meows);
                    break;
                case WardrobeCategory.TURRET:
                    items = new List<ScriptableWardrobeItem>(UniverseWardrobe.Turrets);
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
                
                bool isOwned = PlayerCharacterWardrobe.ContainsId(wardrobeItem.Id);
                wardrobeItemUI.Inflate(wardrobeItem, isOwned, WardrobeOptionsPanel);
            }
        }

        public WardrobeCategory GetActiveCategory()
        {
            return _activeCategory;
        }

        

        private void Clear()
        {
            foreach (Transform child in ItemScrollboxContentRoot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}