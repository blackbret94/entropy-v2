using TanksMP;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class CameraController : MonoBehaviour
    {
        public FollowTarget camFollow { get; set; }

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
        }

        public void SetNormalCam()
        {
            camFollow.SetNormalCam();
        }

        public void HideMask(bool b)
        {
            camFollow.HideMask(b);
        }
    }
}