using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Entropy.Scripts.Player
{
    public class MovementController : MonoBehaviour
    {
        // Cached fields
        private TanksMP.Player _player;
        private GameManager _gameManager;
        private CameraController _cameraController;
        private StatusEffectController _statusEffectController;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _player = GetComponent<TanksMP.Player>();
            _gameManager = GameManager.GetInstance();
            _cameraController = GetComponent<CameraController>();
            _statusEffectController = GetComponent<StatusEffectController>();
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        //moves rigidbody in the direction passed in
        public void Move(Vector2 direction = default(Vector2))
        {
            //if direction is not zero, rotate player in the moving direction relative to camera
            if (direction != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y))
                                            * Quaternion.Euler(0, _cameraController.camTransform.eulerAngles.y, 0);

                float rotationSpeed = Time.deltaTime * 450;
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRotation,
                    rotationSpeed);
            }

            //create movement vector based on current rotation and speed
            float movementSpeed = ((_player.moveSpeed + _statusEffectController.MovementSpeedModifier) *
                                   _statusEffectController.MovementSpeedMultiplier);
            // Vector3 velocity = transform.forward * movementSpeed;
            Vector3 velocity = new Vector3(direction.x, 0, direction.y) * movementSpeed;

            //apply vector to rigidbody position
            _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, velocity, _player.acceleration);
        }


        //on movement drag ended
        public void MoveEnd()
        {
            //reset rigidbody physics values
            // rb.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        //rotates turret to the direction passed in
        // Never called in PlayerBot
        public void RotateTurret(Vector2 direction = default(Vector2))
        {
            if (_player is PlayerBot)
                return;
            
            //don't rotate without values
            if (direction == Vector2.zero)
                return;

            //get rotation value as angle out of the direction we received
            _player.turretRotation = (short)Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y)).eulerAngles.y;
            OnTurretRotation();
        }
        
        //hook for updating turret rotation locally
        //never called in PlayerBot
        public void OnTurretRotation()
        {
            if (_player is PlayerBot)
                return;
            
            //we don't need to check for local ownership when setting the turretRotation,
            //because OnPhotonSerializeView PhotonStream.isWriting == true only applies to the owner
            _player.turret.rotation = Quaternion.Euler(0, _player.turretRotation, 0);
        }

        public void ResetTransform()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
    }
}