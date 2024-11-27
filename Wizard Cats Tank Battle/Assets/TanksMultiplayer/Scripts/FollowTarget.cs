/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Entropy.Scripts.Player;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.Util;

namespace TanksMP
{
    public enum CameraTypes
    {
        normal,
        death
    }
    
    /// <summary>
    /// Camera script for following the player or a different target transform.
    /// Extended with ability to hide certain layers (e.g. UI) while in "follow mode".
    /// </summary>
    public class FollowTarget : MonoBehaviour
    {
        /// <summary>
        /// The camera target to follow.
        /// Automatically picked up in LateUpdate().
        /// </summary>
        public Transform target;
        
        /// <summary>
        /// Layers to hide after calling HideMask().
        /// </summary>
        public LayerMask respawnMask;

        /// <summary>
        /// The clamped distance in the x-z plane to the target.
        /// </summary>
        public float distance = 10.0f;

        public float distanceDeathCam = 8.0f;
        
        /// <summary>
        /// The clamped height the camera should be above the target.
        /// </summary>
        public float height = 5.0f;

        public float heightDeathCam = 3.0f;
        public CameraTypes camMode = 0;
        
        // Zoom
        [Header("Zoom")] public PlayerInputController PlayerInputController;
        public float zoomTime; // Amount of time it takes to do a full zoom, not lined up with seconds
        [FormerlySerializedAs("minZoom")] public float minHeight = 20f; // Minimum zoom level (FOV or orthographic size)
        [FormerlySerializedAs("maxZoom")] public float maxHeight = 60f; // Maximum zoom level (FOV or orthographic size)
        public float minDistance = 10f;
        public float maxDistance = 20f;
        
        private float _currentZoomDistance;
        private float _currentZoomHeight;
        private float _zoomSpeedHeight;
        private float _zoomSpeedDistance;
        private float _targetZoomHeight;
        private float _targetZoomDistance;

        [Header("Springbox Clipping Prevention")]
        public float smoothingSpeed = 10f;
        public float minClampDistance = 5f;
        public LayerMask collisionMask;

        /// <summary>
        /// Reference to the Camera component.
        /// </summary>
        [HideInInspector]
        public Camera cam;
        
        /// <summary>
        /// Reference to the camera Transform.
        /// </summary>
        [HideInInspector]
        public Transform camTransform;
        
        
        //initialize variables
        void Start()
        {
            cam = GetComponent<Camera>();
            camTransform = transform;
            _currentZoomHeight = height;
            _currentZoomDistance = distance;

            _targetZoomHeight = _currentZoomHeight;
            _targetZoomDistance = _currentZoomDistance;

            _zoomSpeedHeight = (maxHeight - minHeight) / zoomTime;
            _zoomSpeedDistance = (maxDistance - minDistance) / zoomTime;

            //the AudioListener for this scene is not attached directly to this camera,
            //but to a separate gameobject parented to the camera. This is because the
            //camera is usually positioned above the player, however the AudioListener
            //should consider audio clips from the position of the player in 3D space.
            //so here we position the AudioListener child object at the target position.
            //Remark: parenting the AudioListener to the player doesn't work, because
            //it gets disabled on death and therefore stops playing sounds completely
            Transform listener = GetComponentInChildren<AudioListener>().transform;
            listener.position = transform.position + transform.forward * getDistance();
        }

        public void SetNormalCam()
        {
            camMode = CameraTypes.normal;
        }

        public void SetDeathCam()
        {
            camMode = CameraTypes.death;
        }

        //position the camera in every frame
        void LateUpdate()
        {
            //cancel if we don't have a target
            if (!target)
                return;
            
            HandleZoom();

            //convert the camera's transform angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            //set the position of the camera on the x-z plane to:
            //distance units behind the target, height units above the target
            Vector3 targetPosition = target.position;
            float desiredDistance = getDistance();
            
            Vector3 desiredPosition = targetPosition - currentRotation * Vector3.forward * Mathf.Abs(desiredDistance);
            desiredPosition.y = targetPosition.y + Mathf.Abs(getHeight());
            
            // Check for obstacles
            // RaycastHit hit;
            // Vector3 cameraOffset = desiredPosition - targetPosition;
            // if (Physics.Raycast(target.position, cameraOffset.normalized, out hit, desiredDistance,
            //         collisionMask))
            // {
            //     float adjustedDistance = Mathf.Clamp(hit.distance, minClampDistance, desiredDistance);
            //     desiredPosition = target.position + cameraOffset.normalized * adjustedDistance;
            // }

            transform.position = desiredPosition;//Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothingSpeed);

            //look at the target
            transform.LookAt(target);

            //clamp distance
            // transform.position = target.position - (transform.forward * Mathf.Abs(getDistance()));
        }
        
        private void HandleZoom()
        {
            // Get mouse scroll input
            Vector2 scrollInput = PlayerInputController.GetAdapter().GetZoomVector();
            float scrollInputSpeed = scrollInput.y * Time.deltaTime;
            if (scrollInputSpeed != 0)
            {
                // Adjust the zoom level
                _targetZoomHeight = Mathf.Lerp(_targetZoomHeight, 
                    _targetZoomHeight - (scrollInputSpeed * _targetZoomHeight), .8f);
                _targetZoomHeight = Mathf.Clamp(_targetZoomHeight, minHeight, maxHeight);

                _targetZoomDistance = Mathf.Lerp(_targetZoomDistance,
                    _targetZoomDistance - (scrollInputSpeed * _targetZoomDistance), .8f);
                _targetZoomDistance = Mathf.Clamp(_targetZoomDistance, minDistance, maxDistance);
            }

            _currentZoomDistance =
                Mathf.Lerp(_currentZoomDistance, _targetZoomDistance, Time.deltaTime * smoothingSpeed);

            _currentZoomHeight = 
                Mathf.Lerp(_currentZoomHeight, _targetZoomHeight, Time.deltaTime * smoothingSpeed);
        }
        
        /// <summary>
        /// Culls the specified layers of 'respawnMask' by the camera.
        /// </summary>
        public void HideMask(bool shouldHide)
        {
            if(shouldHide) cam.cullingMask &= ~respawnMask;
            else cam.cullingMask |= respawnMask;
        }

        private float getDistance()
        {
            if (camMode == 0)
                return _currentZoomDistance;
            if (camMode == CameraTypes.death)
                return distanceDeathCam;

            return _currentZoomDistance;
        }

        private float getHeight()
        {
            if (camMode == 0)
                return _currentZoomHeight;
            if (camMode == CameraTypes.death)
                return heightDeathCam;

            return _currentZoomHeight;
        }
    }
}