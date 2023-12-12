using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Spells
{
    public class SpellField : MonoBehaviour
    {
        public CapsuleCollider Collider;
        public ParticleSystem ParticleSystem;
        
        private List<Player> _playersInZone = new ();
        
        private float _tickTimeS = 1f;
        private float _lastTickS;

        private Player _caster;
        private SpellData _spell;
        private int _teamIndex;

        private float _spawnTime;

        private GameObject _spawnedParticleEffect;
        
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

            if (_spell.EffectToSpawn)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                _spawnedParticleEffect =
                    PoolManager.Spawn(_spell.EffectToSpawn, transform.position, rotation);

                _spawnedParticleEffect.transform.parent = transform;
            }
            
            // ParticleSystem.MainModule mainModule = ParticleSystem.main;
            // mainModule.startLifetime = _spell.Radius/5;
        }

        private void Update()
        {
            // Only execute on the server
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            // Destroy if expired
            if(_spawnTime + _spell.TTL < Time.time)
                PoolManager.Despawn(gameObject);
            
            // Destroy if the caster is dead
            if(!_caster.IsAlive)
                PoolManager.Despawn(gameObject);

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

        private void Tick()
        {
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
            
            // check for player
            Player player;
            if ((player = other.gameObject.GetComponent<Player>()) != null)
            {
                _playersInZone.Add(player);
            }
            
            // check for bullet
            Bullet bullet;
            if ((bullet = other.gameObject.GetComponent<Bullet>()) != null)
            {
                HandleBulletTriggerEnter(bullet);
            }
        }

        private void HandleBulletTriggerEnter(Bullet bullet)
        {
            if (bullet.owner != null)
            {
                if (bullet.owner.GetPhotonView().GetTeam() == _teamIndex)
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
    }
}