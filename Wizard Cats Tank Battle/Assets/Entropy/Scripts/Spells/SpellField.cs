using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.Spells
{
    public class SpellField : MonoBehaviour
    {
        public CapsuleCollider Collider;
        public AudioSource AudioSource;
        
        private List<PlayerController> _playersInZone = new ();
        
        private float _tickTimeS = 1f;
        private float _lastTickS;

        private PlayerController _caster;
        private SpellData _spell;
        private int _teamIndex;

        private float _spawnTime;

        private GameObject _spawnedParticleEffect;

        private bool _setToDespawn;
        
        public void Init(PlayerController caster, SpellData spellData)
        {
            // clear
            if (_spawnedParticleEffect != null)
            {
                PoolManager.Despawn(_spawnedParticleEffect);
            }
            
            _playersInZone.Clear();
            
            // set new values
            _caster = caster;
            _teamIndex = caster.TeamIndex;
            
            _spell = spellData;
            Collider.radius = _spell.Radius;

            _spawnTime = Time.time;
            _setToDespawn = false;

            if (_spell.EffectToSpawn)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                _spawnedParticleEffect =
                    PoolManager.Spawn(_spell.EffectToSpawn, caster.transform.position+Vector3.up*.1f+spellData.CastEffectOffset, rotation);

                _spawnedParticleEffect.transform.parent = transform;
                
                GetParticleSystem().Play(true);
            }
        }

        private void Update()
        {
            // Destroy if expired or if the caster is dead
            if (_spawnTime + _spell.TTL < Time.time || !_caster.IsAlive)
            {
                if (!_setToDespawn)
                {
                    PoolManager.Despawn(gameObject, 1f);
                    ParticleSystem particleSystem = GetParticleSystem();
                    if(particleSystem)
                        particleSystem.Stop(true);
                    
                    _setToDespawn = true;
                }
                return;
            }

            // Follow caster if not stationary
            if (!_spell.IsStationary)
            {   
                Vector3 casterPos = _caster.transform.position;
                transform.position = new Vector3(casterPos.x, transform.position.y, casterPos.z);
            }

            // Apply effects
            if (_lastTickS + _tickTimeS <= Time.time)
            {
                Tick();
                _lastTickS = Time.time;
            }
        }

        private ParticleSystem GetParticleSystem()
        {
            if (_spawnedParticleEffect == null)
                return null;

            ParticleSystem ps = _spawnedParticleEffect.GetComponent<ParticleSystem>();

            if (ps == null)
                return null;

            return ps;
        }

        private void Tick()
        {
            CleanList();
            
            // iterate over players
            foreach (PlayerController player in _playersInZone)
            {
                if(!player.IsAlive)
                    continue;
                
                if (player.TeamIndex == _teamIndex)
                {
                    // add boon
                    if(_spell.ActiveStatusEffectAllies != null)
                        player.ApplyStatusEffect(_spell.ActiveStatusEffectAllies.Id, _caster);
                }
                else
                {
                    // add curse
                    if(_spell.ActiveStatusEffectEnemies != null)
                        player.ApplyStatusEffect(_spell.ActiveStatusEffectEnemies.Id, _caster);
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Check for player
            PlayerController playerController;
            if ((playerController = other.gameObject.GetComponent<PlayerController>()) != null)
            {
                _playersInZone.Add(playerController);
                
                bool playerIsAlly = playerController.TeamIndex == _teamIndex;

                // VFX
                if (_spell.FieldCollisionVfxCharacter)
                {
                    if ((playerIsAlly && _spell.VfxPlayForAllies) || (!playerIsAlly && _spell.VfxPlayForEnemies))
                    {
                        GameObject spawnedEffect = PoolManager.Spawn(_spell.FieldCollisionVfxCharacter,
                            other.transform.position, Quaternion.identity);
                        PoolManager.Despawn(spawnedEffect, 2f);
                    }
                }

                // Audio
                if (_spell.FieldHitSfxCharacter)
                {
                    if ((playerIsAlly && _spell.AudioPlayForAllies) || (!playerIsAlly && _spell.AudioPlayForEnemies))
                    {
                        AudioManager.Play3D(_spell.FieldHitSfxCharacter, playerController.transform.position);
                    }
                }
            }
            
            // Check for bullet
            Projectile projectile;
            if ((projectile = other.gameObject.GetComponent<Projectile>()) != null)
            {
                HandleBulletTriggerEnter(projectile);
                
                bool bulletIsAlly = projectile.GetTeam() == _teamIndex;
                
                // VFX
                if (_spell.FieldCollisionVfxProjectile)
                {
                    if ((bulletIsAlly && _spell.VfxPlayForAllies) || (!bulletIsAlly && _spell.VfxPlayForEnemies))
                    {
                        GameObject spawnedEffect = PoolManager.Spawn(_spell.FieldCollisionVfxCharacter,
                            other.transform.position, Quaternion.identity);
                        PoolManager.Despawn(spawnedEffect, 2f);
                    }
                }

                // Audio
                if (_spell.FieldHitSfxProjectile)
                {
                    if ((bulletIsAlly && _spell.AudioPlayForAllies) || (!bulletIsAlly && _spell.AudioPlayForEnemies))
                    {
                        AudioManager.Play3D(_spell.FieldHitSfxProjectile, projectile.transform.position);
                    }
                }
            }
        }

        private void HandleBulletTriggerEnter(Projectile projectile)
        {
            if (projectile.owner != null)
            {
                if (projectile.GetTeam() == _teamIndex)
                {
                    if (_spell.IncreaseAlliedProjectileSpeedWhileActive > 0)
                    {
                        // Increase speed of allies projectiles
                        projectile.SetSpeed(projectile.GetBaseSpeed() + _spell.IncreaseAlliedProjectileSpeedWhileActive);
                    }
                }
                else
                {
                    if (_spell.DestroyEnemyProjectilesWhileActive)
                    {
                        // Destroy enemy projectiles
                        PoolManager.Despawn(projectile.gameObject);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Check for player
            PlayerController playerController;
            if ((playerController = other.gameObject.GetComponent<PlayerController>()) != null)
            {
                _playersInZone.Remove(playerController);
            }
        }
        
        private void CleanList()
        {
            HashSet<PlayerController> playersInBoundsCopy = new HashSet<PlayerController>(_playersInZone);
            
            foreach (PlayerController player in playersInBoundsCopy)
            {
                if (player == null)
                {
                } else if (!player.IsAlive)
                {
                    _playersInZone.Remove(player);
                }
            }
        }
    }
}