using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    ///#IGNORE
    public class Example_BHELVerbose : VRG_Base
    {
        [SerializeField]
        private Text m_Text = null;

        private void Awake()
        {
            this.Colorize();
        }

        private void Colorize()
        {
            if (this.m_Text != null)
            {
                this.m_Text.text = "<color=" + VRG.GetEnumColor(this.verbose) + ">" + this.verbose + "</color>";
            }
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            switch (this.verbose)
            {
                case ENUM_Verbose.NONE:
                    this.Logs("ENUM_Verbose.NONE", ENUM_Verbose.NONE);
                    break;
                case ENUM_Verbose.ERROR:
                    this.Logs("ENUM_Verbose.ERROR", ENUM_Verbose.ERROR);
                    break;
                case ENUM_Verbose.WARNING:
                    this.Logs("ENUM_Verbose.WARNING", ENUM_Verbose.WARNING);
                    break;
                case ENUM_Verbose.LOGS:
                    this.Logs("ENUM_Verbose.LOGS", ENUM_Verbose.LOGS);
                    break;
                case ENUM_Verbose.DEBUG:
                    this.Logs("ENUM_Verbose.DEBUG", ENUM_Verbose.DEBUG);
                    break;
                case ENUM_Verbose.ALL:
                    this.Logs("ENUM_Verbose.ALL", ENUM_Verbose.ALL);
                    break;
            }

            this.m_Verbose++;

            if (this.m_Verbose > ENUM_Verbose.ALL)
            {
                this.m_Verbose = ENUM_Verbose.NONE;
            }

            this.Colorize();

            yield return null;
        }

    }
}