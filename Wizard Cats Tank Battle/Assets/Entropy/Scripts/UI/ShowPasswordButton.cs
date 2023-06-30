using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class ShowPasswordButton : MonoBehaviour
    {
        public InputField[] PasswordFields;

        public void ToggleShowPassword(bool showPassword)
        {
            foreach (var passwordField in PasswordFields)
            {
                passwordField.contentType =
                    showPassword ? InputField.ContentType.Standard : InputField.ContentType.Password;
                
                passwordField.ForceLabelUpdate();
            }
        }
    }
}