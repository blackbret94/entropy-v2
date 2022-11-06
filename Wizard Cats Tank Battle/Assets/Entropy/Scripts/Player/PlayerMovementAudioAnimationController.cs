using System.Collections;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class PlayerMovementAudioAnimationController : MonoBehaviour
    {
        public ParticleSystem MovementParticleSystem;
        public ParticleSystem RotationParticleSystem;

        public Transform TurretTransform;

        private Vector3 _lastMovement;
        private Vector3 _lastRotation;
        
        private const float MinDistance = .001f;
        private const float RefreshS = .01f;
        private const float MinTimeToAutoRotateS = 1f;

        private void Start()
        {
            _lastMovement = transform.position;
            _lastRotation = TurretTransform.rotation.eulerAngles;
            StartCoroutine(Loop());
        }

        private IEnumerator Loop()
        {
            while (true)
            {
                OneTimeStep();
                yield return new WaitForSeconds(RefreshS + Random.Range(0f, .02f));
            }
        }

        private void OneTimeStep()
        {
            Vector3 thisMovement = transform.position;
            Vector3 thisRotation = TurretTransform.rotation.eulerAngles;
            
            if(Vector3.Distance(_lastMovement, thisMovement) > MinDistance)
                HasMoved();
            else
                HasNotMoved();
            
            if(Vector3.Distance(_lastRotation, thisRotation) > MinDistance)
                HasRotated();
            else
                HasNotRotated();

            _lastMovement = thisMovement;
            _lastRotation = thisRotation;
        }
        
        private void HasMoved()
        {
            if(!MovementParticleSystem.isEmitting)
                MovementParticleSystem.Play();
        }

        private void HasNotMoved()
        {
            if(MovementParticleSystem.isEmitting)
                MovementParticleSystem.Stop();
        }

        private void HasRotated()
        {
            if(!RotationParticleSystem.isEmitting)
                RotationParticleSystem.Play();
        }

        private void HasNotRotated()
        {
            if(RotationParticleSystem.isEmitting)
                RotationParticleSystem.Stop();
        }
    }
}