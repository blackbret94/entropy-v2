using System.Collections;

using UnityEngine;

namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class controls the mechanics of the example DDuA/03 Spawner
    /// Custom the duration and number of random spawns before they dissapears
    /// </summary>
    public class VRG_DDuA_CatalogueIdle : VRG_Base
    {
        [Tooltip("The value to modify, if true it will start the Idle Download")]
        [SerializeField]
        private bool m_Value = true;


        public VRG_DDuA_CatalogueIdle()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = true;
        }

        ///#IGNORE
        protected override IEnumerator Do()
        {
            while (VRG_DDuA.Instance == null)
            {
                yield return null;
            }

            VRG_DDuA.Instance.isCatalogueIdle = this.m_Value;

            yield return null;
        }

    }
}