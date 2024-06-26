using UnityEngine;
using Vashta.Entropy.IO;

namespace Entropy.Scripts.Player
{
    public class PlayerAimGraphic : MonoBehaviour
    {
        public SpriteRenderer Graphic;
        public float ToggleRate = .5f;
        public Transform TurretTransform;
        public float RotationTolerance = 10;

        private bool _disabled = false;
        private bool ShouldShow = false;
        private bool _alwaysOn = false;
        private float _lastRotation;
        
        private float _lastSlowUpdate;
        private float _slowUpdateRate = .25f;
        private float _lastTimeRotationDetected;
        private float _timeBeforeFade = 2f;
        
        private void Start()
        {
            _alwaysOn = SettingsReader.GetAimArrow();
            _lastSlowUpdate = Time.time;

            if (TurretTransform)
            {
                _lastRotation = TurretTransform.eulerAngles.y;
            }

            if (_alwaysOn)
            {
                ShowImage();
            }
            else
            {
                HideImage();
            }
        }

        private void UpdateRotation()
        {
            if (!TurretTransform || _alwaysOn || _disabled)
                return;

            float newRotation = TurretTransform.eulerAngles.y;
            float deltaAngle = Mathf.DeltaAngle(_lastRotation, newRotation);
            
            if (Mathf.Abs(deltaAngle) > RotationTolerance)
            {
                ShowImage();
                _lastRotation = newRotation;
                _lastTimeRotationDetected = Time.time;
            }
            else if (Time.time > _lastTimeRotationDetected + _timeBeforeFade)
            {
                HideImage();
            }
        }

        public void SetColor(Color color)
        {
            if (!Graphic || _disabled)
                return;

            Graphic.color = color;
        }

        public void RefreshAlwaysOn()
        {
            _alwaysOn = SettingsReader.GetAimArrow();
            
            if (_alwaysOn)
            {
                ShowImage();
            }
        }

        private void ShowImage()
        {
            if (_disabled)
                return;
            
            ShouldShow = true;
        }

        private void HideImage()
        {
            if (_alwaysOn)
                return;
            
            ShouldShow = false;
        }

        private void Update()
        {
            if (_disabled)
                return;
            
            if (Time.time - _lastSlowUpdate > _slowUpdateRate)
            {
                SlowUpdate();
            }
            
            if (!Graphic)
                return;

            // Adjust image alpha
            Color color = Graphic.color;
            float alpha = color.a;
            
            if (ShouldShow || _alwaysOn)
            {
                if (alpha < 1f)
                {
                    color.a += Time.deltaTime / ToggleRate;
                }
            }
            else
            {
                if (alpha > 0f)
                {
                    color.a -= Time.deltaTime / ToggleRate;
                }
            }
            
            Graphic.color = color;
        }

        private void SlowUpdate()
        {
            _lastSlowUpdate = Time.time;
            UpdateRotation();
        }

        public void Disable()
        {
            ShouldShow = false;
            _alwaysOn = false;
            _disabled = true;
            
            Color color = Graphic.color;
            color.a = 0;
            Graphic.color = color;
        }
    }
}