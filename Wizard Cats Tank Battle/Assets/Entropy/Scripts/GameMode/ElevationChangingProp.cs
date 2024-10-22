using UnityEngine;

namespace Vashta.Entropy.GameMode
{
    public class ElevationChangingProp : MonoBehaviour
    {
        public Transform TopPos;
        public Transform BottomPos;
        
        private float _topY;
        private float _bottomY;

        private float _targetY;

        private bool _hasInit;

        private void Init()
        {
            if (_hasInit)
                return;
            
            _topY = TopPos.position.y;
            _bottomY = BottomPos.position.y;
            _targetY = _bottomY;

            _hasInit = true;
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            Vector3 pos = transform.position;
            
            // If already close, skip
            if (Mathf.Abs(pos.y - _targetY) < .01)
                return;

            float newY = Mathf.Lerp(pos.y, _targetY, Time.deltaTime*5);

            if (Mathf.Abs(newY - _targetY) > .01)
            {
                // Move towards target
                transform.position = new Vector3(pos.x, newY, pos.z);
            }
            else
            {
                // Clamp to target
                transform.position = new Vector3(pos.x, _targetY, pos.z);
            }
        }
        
        public void SetElevation(float percentage)
        {
            Init();

            percentage = Mathf.Abs(percentage);
            
            _targetY = Mathf.Lerp(_bottomY, _topY, percentage);
            // Vector3 flagPos = transform.position;
            // transform.position = new Vector3(flagPos.x, flagY, flagPos.z);
        }
    }
}