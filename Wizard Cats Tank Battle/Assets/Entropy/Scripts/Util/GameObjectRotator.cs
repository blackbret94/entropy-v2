using System;
using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class GameObjectRotator: MonoBehaviour
    {
        public float speed;
        private Transform _transform;

        private void Start()
        {
            _transform = gameObject.transform;
        }
        
        private void Update()
        {
            _transform.Rotate(0,speed,0);
        }
    }
}