using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class SetGameObjectsActiveOnStart : MonoBehaviour
    {
        public List<GameObject> GameObjectsToEnable;
        public List<GameObject> GameObjectsToDisable;

        public bool delay = false;
        public float delayS = 0f;

        private void Start()
        {
            if (!delay)
            {
                Toggle();
            }
            else
            {
                Invoke("Toggle", delayS);
            }
        }

        private void Toggle()
        {
            foreach (var go in GameObjectsToEnable)
            {
                go.SetActive(true);
            }

            foreach (var go in GameObjectsToDisable)
            {
                go.SetActive(false);
            }
        }
    }
}