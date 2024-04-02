using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// Exit the aplication
    /// </summary>
    public class VRG_Exit : VRG_Base
    {
        protected override IEnumerator Do()
        {
#if UNITY_EDITOR
            this.Logs("Application.Quit();", ENUM_Verbose.NONE);
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // May the force be with you
            Application.Quit();
#endif
            // go to next frame
            yield return null;
        }

    }
}