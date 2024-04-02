using System.Collections;

namespace VrGamesDev
{
    /// <summary>
    /// This class makes a class not destroyable on load <a href="https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html"><em><strong>Object.DontDestroyOnLoad(this);</strong></em></a>
    /// </summary>
    public class VRG_DontDestroyOnLoad : VRG_Base
    {
        protected override IEnumerator Do()
        {
            // i follow my own rules
            this.transform.SetParent(null);

            // ... and I will live forever
            DontDestroyOnLoad(this);

            yield return null;
        }
    }
}