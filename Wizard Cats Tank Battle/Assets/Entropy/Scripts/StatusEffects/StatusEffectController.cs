using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Character;

namespace Vashta.Entropy.StatusEffects
{
    public class StatusEffectController : MonoBehaviour
    {
        public StatusEffectDirectory StatusEffectDirectory;
        private Player _player;
        private PlayerStatusEffectVisualizer _visualizer;
        
        private List<StatusEffect> _statusEffects = new();
        private SortedSet<string> _indexedIds = new();
        private bool _dirtyFlag;
        private float _movementSpeedModifierCached = 0f;
        private float _movementSpeedMultiplierCached = 1f;
        private float _damageOutputModifierCached = 1f;
        private float _defenseModifierCached = 1f;
        private float _healthPerSecondCached = 0f;
        private float _attackRateModifierCached = 1f;
        private float _spikeDamageModifierCached = 0f;
        private bool _isReflectiveCached = false;

        private const float _refreshRateS = .5f;
        private float _lastRefresh = 0;
        private Player _lastDotAppliedBy;
        
        public List<StatusEffect> StatusEffects => _statusEffects;

        public float MovementSpeedModifier => _movementSpeedModifierCached;
        public float MovementSpeedMultiplier => _movementSpeedMultiplierCached;
        public float DamageOutputModifier => _damageOutputModifierCached;
        public float DefenseModifier => _defenseModifierCached; // TODO: Implement Defense
        public float HealthPerSecond => _healthPerSecondCached;
        public float AttackRateModifier => _attackRateModifierCached;
        public float SpikeDamageModifier => _spikeDamageModifierCached;
        public bool IsReflective => _isReflectiveCached;
        public Player LastDotAppliedBy => _lastDotAppliedBy;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _visualizer = GetComponent<PlayerStatusEffectVisualizer>();
        }
        
        /// <summary>
        /// Server only, call RPC on all clients
        /// </summary>
        /// <param name="statusEffectId"></param>
        /// <param name="owner"></param>
        public void AddStatusEffect(string statusEffectId, Player owner)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _player.photonView.RPC("RPCAddStatusEffect", RpcTarget.All, statusEffectId, owner?owner.GetId():null);
            }
        }
        
        /// <summary>
        /// Called on all clients to add status effect
        /// </summary>
        /// <param name="statusEffectId"></param>
        /// <param name="playerId"></param>
        [PunRPC]
        public void RPCAddStatusEffect(string statusEffectId, int playerId)
        {
            Player owner = Player.GetPlayerById(playerId);
            StatusEffect statusEffect = new StatusEffect(this, statusEffectId, owner);

            if(_player.IsLocal)
                GameManager.GetInstance().ui.PowerUpPanel.SetText(statusEffect.Title(),statusEffect.Description(), statusEffect.Color());
            
            StatusEffect existingEffect = StatusEffectAlreadyExists(statusEffect.Id());

            if (existingEffect == null)
            {
                _statusEffects.Add(statusEffect);
                _indexedIds.Add(statusEffect.Id());
                _dirtyFlag = true;
            }
            else
            {
                existingEffect.SetExpiration();
            }
        }

        public void ClearStatusEffects()
        {
            _statusEffects.Clear();
            _indexedIds.Clear();
        }

        private void Update()
        {
            if(Time.time + _refreshRateS >= _lastRefresh)
                CheckLifeOfStatusEffects();
            
            if(_dirtyFlag)
                RefreshCache();

            _lastRefresh = Time.time;
        }
        
        private void CheckLifeOfStatusEffects()
        {
            // Duplicate to safely iterate
            List<StatusEffect> statusEffects = new List<StatusEffect>(_statusEffects);

            foreach (StatusEffect statusEffect in statusEffects)
            {
                if (statusEffect.HasExpired())
                    RemoveStatusEffect(statusEffect);
            }
        }

        private void RemoveStatusEffect(StatusEffect statusEffect)
        {
            _statusEffects.Remove(statusEffect);
            _indexedIds.Remove(statusEffect.Id());
            _dirtyFlag = true;
        }

        private void RefreshCache()
        {
            _movementSpeedModifierCached = 0;
            _movementSpeedMultiplierCached = 1f;
            _damageOutputModifierCached = 1;
            _defenseModifierCached = 1;
            _healthPerSecondCached = 0f;
            _attackRateModifierCached = 1f;
            _spikeDamageModifierCached = 0f;
            _isReflectiveCached = false;

            foreach (var statusEffect in _statusEffects)
            {
                _movementSpeedModifierCached += statusEffect.MovementSpeedModifier();
                _movementSpeedMultiplierCached *= statusEffect.MovementSpeedMultiplier();
                _damageOutputModifierCached *= statusEffect.DamageOutputMultiplier();
                _defenseModifierCached *= statusEffect.DefenseModifier();
                _healthPerSecondCached += statusEffect.HealthPerSecond();
                _attackRateModifierCached *= statusEffect.AttackRateMultiplier();
                _spikeDamageModifierCached += statusEffect.SpikeDamageModifier();

                if (statusEffect.HealthPerSecond() < 0)
                    _lastDotAppliedBy = statusEffect.OriginPlayer();

                if (statusEffect.IsReflective())
                    _isReflectiveCached = true;
            }

            _visualizer.Refresh(_indexedIds);
            _dirtyFlag = false;
        }

        private StatusEffect StatusEffectAlreadyExists(string id)
        {
            foreach (var statusEffect in _statusEffects)
            {
                if (statusEffect.Id() == id)
                    return statusEffect;
            }

            return null;
        }
    }
}