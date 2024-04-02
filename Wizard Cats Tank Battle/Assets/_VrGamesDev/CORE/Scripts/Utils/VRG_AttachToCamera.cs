using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// On enable this gameobject attach itself to the main camera 
    /// </summary>
    public class VRG_AttachToCamera : VRG_Base
    {
        protected override IEnumerator Do()
        {
            // search for camera and attach it
            this.transform.SetParent(Camera.main.transform, false);

            // go to next frame
            yield return null;
        }

    }
}