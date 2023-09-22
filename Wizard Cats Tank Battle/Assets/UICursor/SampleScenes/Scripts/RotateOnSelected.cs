using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Cooldhands.UICursor.Example
{
    public class RotateOnSelected : MonoBehaviour
    {
        public Vector3 RotateVector =  new Vector3(50,0,0);

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == this.gameObject)
            {
                transform.Rotate (RotateVector * Time.deltaTime); //rotates 50 degrees per second around z axis
            }
        }
    }
}
