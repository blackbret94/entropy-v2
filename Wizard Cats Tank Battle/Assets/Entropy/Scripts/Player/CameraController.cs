using TanksMP;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class CameraController : MonoBehaviour
    {
        public FollowTarget camFollow { get; set; }
        public Camera HUDCamera { get; set; }

        public Transform camTransform => camFollow.camTransform;

        private void Awake()
        {
            if (Camera.main != null)
            {
                camFollow = Camera.main.GetComponent<FollowTarget>();
            }
            else
            {
                Debug.LogWarning("No MainCamera");
            }
            
            if (HUDCamera == null)
            {
                HUDCamera = GameObject.Find("HUD Camera").GetComponent<Camera>();
            }

            if (HUDCamera == null)
            {
                Debug.LogError("Could not find HUD Camera!");
            }
        }

        public void FollowKiller(GameObject killedBy)
        {
            if (killedBy != null)
            {
                SetTarget(killedBy.transform);
                SetDeathCam();
            }
                
            //hide input controls and other HUD elements
            HideMask(true);
        }

        public void FollowPlayer(Transform target)
        {
            SetTarget(target);
            SetNormalCam();
            HideMask(false);
        }
        
        public void SetTarget(Transform target)
        {
            camFollow.target = target;
        }

        public void SetDeathCam()
        {
            camFollow.SetDeathCam();
            if(HUDCamera)
                HUDCamera.gameObject.SetActive(false);
        }

        public void SetNormalCam()
        {
            camFollow.SetNormalCam();
            if(HUDCamera)
                HUDCamera.gameObject.SetActive(true);
        }

        public void HideMask(bool b)
        {
            camFollow.HideMask(b);
        }
    }
}