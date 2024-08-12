using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputIcons
{
    public class II_ChangePlayerInputButton : MonoBehaviour
    {
        public void ChangeInput()
        {
            FindObjectOfType<II_ChangePlayerInput>()?.ChangeInput();
        }
    }
}

