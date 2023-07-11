using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.MapSelection
{
    public class MapSelectionCheckbox : MonoBehaviour
    {
        [FormerlySerializedAs("MapDefinitionData")] public MapDefinition mapDefinition;

        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI SubText;
        
        public Image MapImage;
        public Image SelectedFrame;
        public bool StartSelected;
        public bool IsRandom;

        private void Start()
        {
            if (mapDefinition != null)
            {
                MapImage.sprite = mapDefinition.MapPreviewImage;
                TitleText.text = mapDefinition.Title;
                SubText.text = mapDefinition.Description;
            }

            Toggle(StartSelected);   
        }
        
        public void Toggle(bool setActive)
        {
            SelectedFrame.gameObject.SetActive(setActive);
        }
    }
}