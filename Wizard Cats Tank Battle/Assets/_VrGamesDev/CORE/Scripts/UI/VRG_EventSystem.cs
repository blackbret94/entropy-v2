using System.Collections;

namespace VrGamesDev
{
    /// <summary>
    /// This class makes a class not destroyable on load <a href="https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html"><em><strong>Object.DontDestroyOnLoad(this);</strong></em></a>
    /// </summary>
    public class VRG_EventSystem : VRG_Base
    {
        /// <summary>
        /// Singleton pattern, Instance is the unique variable in the scene from this class
        /// </summary>
        public static VRG_EventSystem Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // i follow my own rules
                this.transform.SetParent(null);

                // ... and I will live forever
                DontDestroyOnLoad(this);
            }
            else
            {
                // I am not the one the prophecy said ... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }



        protected override IEnumerator Do() { yield return null; }
    }
}