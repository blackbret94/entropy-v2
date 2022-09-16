using TMPro;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class CharacterWardrobeSelector: GamePanel
    {
        public PlayerCharacterWardrobe PlayerCharacterWardrobe;
        public CharacterAppearance CharacterAppearance;
        public WardrobeCategory WardrobeCategory;
        public TextMeshProUGUI SelectorText;

        private void Start()
        {
            Refresh();
        }

        public override void Refresh()
        {
            base.Refresh();
            
            switch (WardrobeCategory)
            {
                case WardrobeCategory.HAT:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.Hat.HatId, PlayerCharacterWardrobe.Hats.Count));
                    break;
                
                case WardrobeCategory.BODY_TYPE:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.Body.BodyTypeId, PlayerCharacterWardrobe.BodyTypes.Count));
                    break;
                
                case WardrobeCategory.SKIN:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.Skin.SkinId, PlayerCharacterWardrobe.GetBodyTypeById(CharacterAppearance.Body.BodyTypeId).SkinOptions.Count));
                    break;
                
                case WardrobeCategory.CART:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.Cart.CartId, PlayerCharacterWardrobe.Carts.Count));
                    break;
            }
        }
        
        public void NextItem()
        {
            switch (WardrobeCategory)
            {
                case WardrobeCategory.HAT:
                    UpdateText(CharacterAppearance.NextHat());
                    break;
                
                case WardrobeCategory.BODY_TYPE:
                    UpdateText(CharacterAppearance.NextBodyType());
                    break;
                
                case WardrobeCategory.SKIN:
                    UpdateText(CharacterAppearance.NextSkin());
                    break;
                
                case WardrobeCategory.CART:
                    UpdateText(CharacterAppearance.NextCart());
                    break;
            }
        }

        public void PreviousItem()
        {
            switch (WardrobeCategory)
            {
                case WardrobeCategory.HAT:
                    UpdateText(CharacterAppearance.PrevHat());
                    break;
                
                case WardrobeCategory.BODY_TYPE:
                    UpdateText(CharacterAppearance.PrevBodyType());
                    break;
                
                case WardrobeCategory.SKIN:
                    UpdateText(CharacterAppearance.PrevSkin());
                    break;
                
                case WardrobeCategory.CART:
                    UpdateText(CharacterAppearance.PrevCart());
                    break;
            }
        }

        private void UpdateText(CharacterWardrobeSelectorData data)
        {
            SelectorText.text = $"{data.ItemIndex}/{data.ItemCount}";
        }
    }
}