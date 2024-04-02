using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;


namespace VrGamesDev.DDuA
{
    ///#IGNORE
    public class Example_VRG_AddressableToBhel : VRG_Base
    {
        [Tooltip("asdasdasd")]
        [SerializeField]
        protected VRG_Addressable m_Addressable = null;

        [Tooltip("asdasdasd")]
        [SerializeField]
        protected Text m_Text = null;


        private void Start()
        {
            if (this.m_Addressable != null)
            {
                this.m_Addressable.WhenBHEL += M_Addressable_WhenBHEL;
            }
        }

        private void M_Addressable_WhenBHEL(string valueLocal)
        {
            if (this.m_Text != null)
            {
                this.m_Text.text = valueLocal;
            }
            else
            {
                print(valueLocal);
            }
        }

        private void OnDestroy()
        {
            this.m_Addressable.WhenBHEL -= M_Addressable_WhenBHEL;
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            yield return null;
        }


    }
}