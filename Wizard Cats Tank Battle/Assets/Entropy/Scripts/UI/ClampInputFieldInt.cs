using System;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class ClampInputFieldInt : MonoBehaviour
    {
        public InputField InputField;
        
        public int Max = 12;
        public int Min = 0;

        private void Start()
        {
            InputField.onValueChanged.AddListener(delegate { Clamp(); });
        }

        private void Clamp()
        {
            try
            {
                int value = Int32.Parse(InputField.text);

                value = Mathf.Clamp(value, Min, Max);
                InputField.text = value.ToString();

            }
            catch (FormatException)
            {
                InputField.text = Max.ToString();
            }
        }
    }
}