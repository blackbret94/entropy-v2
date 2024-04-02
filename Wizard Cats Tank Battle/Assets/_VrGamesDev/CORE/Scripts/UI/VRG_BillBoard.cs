using System.Collections;

using UnityEngine;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
using Sirenix.OdinInspector;
#endif

namespace VrGamesDev
{
    //[ExecuteInEditMode]
    public class VRG_BillBoard : VRG_Base
    {
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Component")]
#endif
        [Tooltip("La cámara principal a la cual se va a alinear")]
        [SerializeField] private Camera m_MainCamera;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        [Tooltip("FLAG que determina si se debe de alinear a la camera view")]
        [SerializeField] private bool m_AlignToCamera = true;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        [Tooltip("FLAG: Escala del objeto para mantenerlo al mismo tamaño sin importar la distancia")]
        [SerializeField] private bool m_SameSizeToCamera = true;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowIf("m_SameSizeToCamera")]
        [ToggleGroup("Configuration")]
#endif
        [Tooltip("La escala del objeto, esta se relaciona con la escala original")]
        [SerializeField] private float m_objectScale = 1.0f;










        // set the initial scale, and setup reference camera
        private void Start()
        {
            // buscar y registrar la cámara
            this.m_MainCamera = this.FindMy(this.m_MainCamera, false);
        }

        ///#IGNORE
        protected override IEnumerator Do() { yield return null; }

        // scale object relative to distance from camera plane
        void Update()
        {
            // si la FLAG para cambiar el tamaño esta activa
            if (this.m_SameSizeToCamera && this.m_MainCamera != null)
            {
                // se obtiene el plano ortogonal de la camara 
                Plane plane = new Plane(this.m_MainCamera.transform.forward, this.m_MainCamera.transform.position);

                // se calcula la distancia del plano de la camara al objeto que se va a mencionar
                float fDistance = plane.GetDistanceToPoint(this.transform.position);

                // se cambia la escala del objeto para que este del tamaño adecuado a la camara
                this.transform.localScale = Vector3.one * this.m_objectScale * fDistance;
            }
        }

        //Orient the camera after all movement is completed this frame to avoid jittering
        private void LateUpdate()
        {
            // si la FLAG para alinear a la camara esta activa
            if (this.m_AlignToCamera && this.m_MainCamera != null)
            {
                // Se alinea el objeto su rotación de su forward / up para que siempre este perpendicular a la camara
                this.transform.LookAt
                (
                    this.transform.position + this.m_MainCamera.transform.rotation * Vector3.forward,
                    this.m_MainCamera.transform.rotation * Vector3.up
                );
            }
        }
    }
}