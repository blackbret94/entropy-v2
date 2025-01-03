using Entropy.Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class WardrobeItemUI : MonoBehaviour
    {
        public Image Background;
        public Image Icon;
        public Sprite NotOwnedBackground;
        public GameObject Frame;

        public RarityDictionary RarityDictionary;
        
        private ScriptableWardrobeItem _scriptableWardrobeItem;
        private bool _isOwned;
        private WardrobeOptionsPanel _wardrobeOptionsPanel;
        private SfxController _sfxController;
        
        public void Inflate(ScriptableWardrobeItem scriptableWardrobeItem, bool isOwned, WardrobeOptionsPanel wardrobeOptionsPanel,
            SfxController sfxController)
        {
            _scriptableWardrobeItem = scriptableWardrobeItem;
            _isOwned = isOwned;
            _wardrobeOptionsPanel = wardrobeOptionsPanel;
            _sfxController = sfxController;

            if (isOwned)
            {
                Rarity rarity = scriptableWardrobeItem.Rarity;
                RarityDefinition rarityDefinition = RarityDictionary[rarity];
                
                Background.sprite = rarityDefinition.BackgroundImage;
            }
            else
            {
                Background.sprite = NotOwnedBackground;
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
            Select();
            
            _sfxController.PlayBasicButtonClick();
        }

        public void Select()
        {
            Frame.SetActive(true);
        }

        public void Deselect()
        {
            Frame.SetActive(false);
        }
    }
}