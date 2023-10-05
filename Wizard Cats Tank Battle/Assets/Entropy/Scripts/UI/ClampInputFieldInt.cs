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
            InputField.onEndEdit.AddListener(delegate { ClampInputField(); });
        }

        private void ClampInputField()
        {
            InputField.text = Clamp().ToString();
        }

        private int Clamp()
        {
            try
            {
                int value = Int32.Parse(InputField.text);

                value = Mathf.Clamp(value, Min, Max);
                return value;

            }
            catch (FormatException)
            {
                return Max;
            }
        }

        public int GetClampedValue()
        {
            return Clamp();
        }
    }
}