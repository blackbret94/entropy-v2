using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeItemUI : MonoBehaviour
    {
        public Image Background;
        public Image Icon;

        public Color OwnedColor;
        public Color NotOwnedColor;
        
        private ScriptableWardrobeItem _scriptableWardrobeItem;
        private bool _isOwned;
        private WardrobeOptionsPanel _wardrobeOptionsPanel;

        public void Inflate(ScriptableWardrobeItem scriptableWardrobeItem, bool isOwned, WardrobeOptionsPanel wardrobeOptionsPanel)
        {
            _scriptableWardrobeItem = scriptableWardrobeItem;
            _isOwned = isOwned;
            _wardrobeOptionsPanel = wardrobeOptionsPanel;

            if (isOwned)
            {
                Background.color = OwnedColor;
            }
            else
            {
                Background.color = NotOwnedColor;
            }

            Icon.sprite = scriptableWardrobeItem.Icon;
        }

        public void Clicked()
        {
            if (!_wardrobeOptionsPanel)
            {
                Debug.LogError("Missing connection to WardrobeOptionsPanel");
                return;
            }
            
            _wardrobeOptionsPanel.SelectItem(_scriptableWardrobeItem);
        }
    }
}