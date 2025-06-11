using Entropy.Scripts.Audio;
using TMPro;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.Quickplay
{
    public class QuickplayModeButton : GamePanel
    {
        public QuickplayPanelMode PanelMode;
        
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DescriptionText;
        public Image Sprite;
        public GameModeDefinition ModeDefinition;

        private void Start()
        {
            if (ModeDefinition != null)
            {
                Inflate(ModeDefinition);
            }
            
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                SfxController sfxController = SfxController.GetInstance();
                if(sfxController != null)
                    sfxController.PlayBasicButtonClick();
                
                if(ModeDefinition)
                    PanelMode.StartGameMode(ModeDefinition);
            });
        }
        
        public void Inflate(GameModeDefinition modeDefinition)
        {
            ModeDefinition = modeDefinition;
            
            TitleText.text = modeDefinition.Title;
            DescriptionText.text = modeDefinition.Description;
            Sprite.sprite = modeDefinition.Icon;
            TitleText.color = modeDefinition.Color;
        }
    }
}