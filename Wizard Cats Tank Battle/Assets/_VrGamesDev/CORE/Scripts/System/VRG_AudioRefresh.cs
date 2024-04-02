using System.Collections;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Update the VRG_Audio mute settings of all the groups from the audio mixer
    /// </summary>
    public class VRG_AudioRefresh : VRG_Base
    {
        // Enumerator proxy, it is activated OnEnable
        protected override IEnumerator Do()
        {
            // Let's assume everything is configured properly
            yield return VRG_Audio.IsValid();

            // is it?
            if (VRG_Audio.Instance != null)
            {
                VRG_Audio.Mute();
            }

            // return, it is like a void
            yield return null;
        }
    }
}