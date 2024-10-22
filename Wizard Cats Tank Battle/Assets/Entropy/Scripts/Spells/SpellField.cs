using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Spells
{
    public class SpellField : MonoBehaviour
    {
        public CapsuleCollider Collider;
        public AudioSource AudioSource;
        
        private List<Player> _playersInZone = new ();
        
        private float _tickTimeS = 1f;
        private float _lastTickS;

        private Player _caster;
        private SpellData _spell;
        private int _teamIndex;

        private float _spawnTime;

        private GameObject _spawnedParticleEffect;

        private bool _setToDespawn;
        
        public void Init(Player caster, SpellData spellData)
        {
            // clear
            if (_spawnedParticleEffect != null)
            {
                PoolManager.Despawn(_spawnedParticleEffect);
            }
            
            _playersInZone.Clear();
            
            // set new values
            _caster = caster;
            _teamIndex = caster.GetView().GetTeam();
            
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
                    GetParticleSystem().Stop(true);
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
            
            // Only execute on the server
            if (!PhotonNetwork.IsMasterClient)
                return;

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
            foreach (Player player in _playersInZone)
            {
                if(!player.IsAlive)
                    continue;
                
                if (player.GetView().GetTeam() == _teamIndex)
                {
                    // add boon
                    if(_spell.ActiveStatusEffectAllies != null)
                        player.ApplyStatusEffect(_spell.ActiveStatusEffectAllies.Id, _caster.GetId());
                }
                else
                {
                    // add curse
                    if(_spell.ActiveStatusEffectEnemies != null)
                        player.ApplyStatusEffect(_spell.ActiveStatusEffectEnemies.Id, _caster.GetId());
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            // Check for player
            Player player;
            if ((player = other.gameObject.GetComponent<Player>()) != null)
            {
                _playersInZone.Add(player);
                
                bool playerIsAlly = player.GetTeam() == _teamIndex;

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
                        AudioManager.Play3D(_spell.FieldHitSfxCharacter, player.transform.position);
                    }
                }
            }
            
            // Check for bullet
            Bullet bullet;
            if ((bullet = other.gameObject.GetComponent<Bullet>()) != null)
            {
                HandleBulletTriggerEnter(bullet);
                
                bool bulletIsAlly = bullet.GetTeam() == _teamIndex;
                
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
                        AudioManager.Play3D(_spell.FieldHitSfxProjectile, bullet.transform.position);
                    }
                }
            }
        }

        private void HandleBulletTriggerEnter(Bullet bullet)
        {
            if (bullet.owner != null)
            {
                if (bullet.GetTeam() == _teamIndex)
                {
                    if (_spell.IncreaseAlliedProjectileSpeedWhileActive > 0)
                    {
                        // Increase speed of allies projectiles
                        bullet.SetSpeed(bullet.GetBaseSpeed() + _spell.IncreaseAlliedProjectileSpeedWhileActive);
                    }
                }
                else
                {
                    if (_spell.DestroyEnemyProjectilesWhileActive)
                    {
                        // Destroy enemy projectiles
                        PoolManager.Despawn(bullet.gameObject);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            // Check for player
            Player player;
            if ((player = other.gameObject.GetComponent<Player>()) != null)
            {
                _playersInZone.Remove(player);
            }
        }
        
        private void CleanList()
        {
            HashSet<Player> playersInBoundsCopy = new HashSet<Player>(_playersInZone);
            
            foreach (Player player in playersInBoundsCopy)
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