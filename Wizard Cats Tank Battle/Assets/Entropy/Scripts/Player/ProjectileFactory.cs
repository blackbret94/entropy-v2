using UnityEngine;
using TanksMP;
using Vashta.Entropy.StatusEffects;


namespace Entropy.Scripts.Player
{
    public class ProjectileFactory
    {
        private GameObject _playerGameObject;
        private StatusEffectController _statusEffectController;

        public ProjectileFactory(GameObject playerGameObject, StatusEffectController statusEffectController)
        {
            _playerGameObject = playerGameObject;
            _statusEffectController = statusEffectController;
        }
        
        public void SpawnProjectile(Vector3 shotCenter, Quaternion syncedRot, ClassDefinition playerClass, float damageModifier = 1)
        {
            if (playerClass == null)
            {
                Debug.LogError("Player class is null");
                return;
            }
            GameObject obj = PoolManager.Spawn(playerClass.Missile, shotCenter, syncedRot);
            Bullet newBullet = obj.GetComponent<Bullet>();
            
            newBullet.SpawnNewBullet();
            newBullet.owner = _playerGameObject;
            newBullet.ClassDefinition = playerClass;
            newBullet.SetDamage(Mathf.CeilToInt(newBullet.GetRawDamage() * _statusEffectController.DamageOutputModifier * damageModifier));
            newBullet.canBuff = !_statusEffectController.BlocksCastingBuffs;
            newBullet.canDebuff = !_statusEffectController.BlocksCastingDebuffs;

            if (_statusEffectController.ProjectileExplodes)
            {
                newBullet.SetExplosionRange(3);
                newBullet.SetMaxTargets(3);
            }

            if (_statusEffectController.ProjectileReflects)
            {
                newBullet.SetMaxBounce(10);
            }

            if (_statusEffectController.ProjectileLifeExtended > 0)
            {
                newBullet.IncreaseDespawnDelay(_statusEffectController.ProjectileLifeExtended);
            }

            if (_statusEffectController.Pierces)
            {
                newBullet.SetPiercing(true);
            }
        }
    }
}