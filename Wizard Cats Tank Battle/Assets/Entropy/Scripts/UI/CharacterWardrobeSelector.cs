using Entropy.Scripts.Player.Inventory;
using TMPro;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.UI
{
    public class CharacterWardrobeSelector: GamePanel
    {
        public PlayerInventory PlayerInventory;
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
            PlayerInventory.Load();
            
            switch (WardrobeCategory)
            {
                case WardrobeCategory.HAT:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.HatIndex+1, PlayerInventory.Hats.Count));
                    break;
                
                case WardrobeCategory.BODY_TYPE:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.BodyIndex+1, PlayerInventory.BodyTypes.Count));
                    break;
                
                case WardrobeCategory.SKIN:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.SkinIndex+1, PlayerInventory.GetBodyTypeByIndex(CharacterAppearance.BodyIndex).SkinOptions.Count));
                    break;
                
                case WardrobeCategory.CART:
                    UpdateText(new CharacterWardrobeSelectorData(CharacterAppearance.CartIndex+1, PlayerInventory.Carts.Count));
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