using System.Collections;

using UnityEngine.AddressableAssets;

namespace VrGamesDev.DDuA
{
    /// <summary>
    /// Exit the aplication
    /// </summary>
    public class VRG_Destroy_Addressable : VRG_Base
    {
        protected override IEnumerator Do()
        {
            // return, it is like a void and wait a frame
            yield return null;
        }

        private void OnDestroy()
        {
            // release the handler
            Addressables.ReleaseInstance(this.gameObject);
        }


    }
}