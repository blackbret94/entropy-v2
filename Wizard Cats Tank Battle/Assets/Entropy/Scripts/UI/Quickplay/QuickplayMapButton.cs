using Entropy.Scripts.Audio;
using TMPro;
using UnityEngine.UI;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.Quickplay
{
    public class QuickplayMapButton : GamePanel
    {
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DescriptionText;
        public Image Sprite;
        public MapDefinition MapDefinition;
        public QuickplayPanelMap QuickplayPanelMap;

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                SfxController sfxController = SfxController.GetInstance();
                if(sfxController != null)
                    sfxController.PlayDramaticButtonClick();
                
                if(MapDefinition)
                    QuickplayPanelMap.StartMap(MapDefinition);
            });
        }
        
        public void Inflate(MapDefinition mapDefinition, QuickplayPanelMap quickplayPanelMap)
        {
            MapDefinition = mapDefinition;
            
            TitleText.text = MapDefinition.Title;
            DescriptionText.text = MapDefinition.Description;
            Sprite.sprite = MapDefinition.MapPreviewImage;
            TitleText.color = MapDefinition.Color;
            
            QuickplayPanelMap = quickplayPanelMap;
        }
    }
}