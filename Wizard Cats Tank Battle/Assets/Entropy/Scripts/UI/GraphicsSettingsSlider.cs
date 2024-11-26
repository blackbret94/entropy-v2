using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.IO;

namespace Vashta.Entropy.UI
{
    public class GraphicsSettingsSlider : MonoBehaviour
    {
        public Slider graphicsSlider;
        public TextMeshProUGUI qualityLabel; // Optional: To display the selected quality name

        void Start()
        {
            // Initialize the slider with the current quality level
            graphicsSlider.maxValue = QualitySettings.names.Length - 1;
            graphicsSlider.value = SettingsReader.GetGraphicsSettings();

            // Update the label (optional)
            UpdateQualityLabel(graphicsSlider.value);

            // Add listener for value change
            // graphicsSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        public void OnSliderValueChanged(float value)
        {
            // Update the label (optional)
            
            QualitySettings.SetQualityLevel(Mathf.RoundToInt(graphicsSlider.value));
            UpdateQualityLabel(value);
        }

        private void UpdateQualityLabel(float value)
        {
            if (qualityLabel != null)
            {
                qualityLabel.text = QualitySettings.names[Mathf.RoundToInt(value)];
            }
        }
    }
}