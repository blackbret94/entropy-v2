using System.Collections;

using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// Scale a GameObject in X,Y,X, simple and effective
    /// It scale from current Scale to the Target Vector3
    /// </summary>
    public class VRG_Position : VRG_Base
    {
        [Tooltip("Target Scale to achieve")]
        [SerializeField]//
        private Vector3 m_Origin = new Vector3(-4.0f, -2.5f, 0.0f);

        [Tooltip("Target Scale to achieve")]
        [SerializeField]//
        private Vector3 m_Target = new Vector3(4.0f, 1.5f, 0.0f);



        /// <summary>
        /// Empty Creator, everything is true and zero
        /// </summary>
        public VRG_Position()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = false;
        }


        // Enumerator proxy
        protected override IEnumerator Do()
        {
            this.transform.position = new Vector3
            (
                Random.Range(this.m_Origin.x, this.m_Target.x),
                Random.Range(this.m_Origin.y, this.m_Target.y),
                Random.Range(this.m_Origin.z, this.m_Target.z)
            );

            yield return null;
        }
    }
}