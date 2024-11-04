using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class GameModeCheckbox : MonoBehaviour
    {
        public GameModeDefinition GameModeDefinition;

        public Image GameModeImage;
        public Image SelectedFrame;
        public bool StartSelected;
        public bool IsRandom;

        public Color SupportedByMapColor = Color.white;
        public Color NotSupportedByMapColor;

        public Image Icon;
        public Button Button;

        private bool _isSupportedByMap;

        private void Start()
        {
            if (GameModeDefinition != null)
            {
                GameModeImage.sprite = GameModeDefinition.Icon;
            }
            
            Toggle(StartSelected);
        }
        
        public void Toggle(bool b)
        {
            SelectedFrame.gameObject.SetActive(b);
        }

        public void SetToMap(MapDefinition mapDefinition)
        {
            if (IsRandom)
                return;
            
            TanksMP.GameMode gameMode = GameModeDefinition.GameMode;

            if (mapDefinition == null || mapDefinition.SupportedGameModes.Contains(gameMode))
            {
                // supported
                _isSupportedByMap = true;
                Button.enabled = true;
                Icon.color = SupportedByMapColor;
            }
            else
            {
                // Not supported
                _isSupportedByMap = false;
                Button.enabled = false;
                Icon.color = NotSupportedByMapColor;
            }
        }

        public bool IsSupportedByMap()
        {
            if (IsRandom)
                return true;
            
            return _isSupportedByMap;
        }
    }
}