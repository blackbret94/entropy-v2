using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeOptionsPanel: GamePanel
    {
        public WardrobeItemBox WardrobeItemBox;
        public WardrobeItemText WardrobeItemText;
        
        private ScriptableWardrobeItem _selectedItem;
        
        public void SelectItem(ScriptableWardrobeItem scriptableWardrobeItem)
        {
            _selectedItem = scriptableWardrobeItem;
            WardrobeItemText.SetItem(scriptableWardrobeItem);
        }

        public void AttemptToPurchaseSelectedItem()
        {
            
        }
    }
}