using Entropy.Scripts.Player.Inventory;
using TMPro;
using Vashta.Entropy.Character;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class ItemCollectedCounterPanel: GamePanel
    {
        public TextMeshProUGUI Text;
        public PlayerInventory PlayerInventory;
        public PlayerCharacterWardrobe Wardrobe;

        public string AllItemsCollectedString = "All items collected!";

        private WardrobeCategory _category;
        
        public void UpdatePanel(WardrobeCategory category)
        {
            _category = category;
            UpdateText();
        }

        private void UpdateText()
        {
            Text.text = GenerateTextString();
        }

        private string GenerateTextString()
        {
            int wardrobeCount = Wardrobe.GetItemCountByCategory(_category);
            int inventoryCount = PlayerInventory.GetItemCountByCategory(_category);

            if (wardrobeCount == inventoryCount)
                return AllItemsCollectedString;
            else
                return "Collected " + inventoryCount + " of " + wardrobeCount;
        }
    }
}