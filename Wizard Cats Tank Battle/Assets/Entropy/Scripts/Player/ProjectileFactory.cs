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
            Projectile newProjectile = obj.GetComponent<Projectile>();
            
            newProjectile.SpawnNewBullet();
            newProjectile.owner = _playerGameObject;
            newProjectile.ClassDefinition = playerClass;
            newProjectile.SetDamage(Mathf.CeilToInt(newProjectile.GetRawDamage() * _statusEffectController.DamageOutputModifier * damageModifier));
            newProjectile.canBuff = !_statusEffectController.BlocksCastingBuffs;
            newProjectile.canDebuff = !_statusEffectController.BlocksCastingDebuffs;

            if (_statusEffectController.ProjectileExplodes)
            {
                newProjectile.SetExplosionRange(3);
                newProjectile.SetMaxTargets(3);
            }

            if (_statusEffectController.ProjectileReflects)
            {
                newProjectile.SetMaxBounce(10);
            }

            if (_statusEffectController.ProjectileLifeExtended > 0)
            {
                newProjectile.IncreaseDespawnDelay(_statusEffectController.ProjectileLifeExtended);
            }

            if (_statusEffectController.Pierces)
            {
                newProjectile.SetPiercing(true);
            }
        }
    }
}