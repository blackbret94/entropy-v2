using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// It shows the correct title according to the Session data difficulty
    /// </summary>
    public class VRG_5sTitle : VRG_SessionData
    {
        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Display the named child and hide the others.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            foreach (Transform child in this.transform)
            {
                if (child.name == this.value)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            // next frame
            yield return null;
        }


    }
}