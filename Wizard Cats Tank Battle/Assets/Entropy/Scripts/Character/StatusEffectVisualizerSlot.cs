using System;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.Character
{
    public class StatusEffectVisualizerSlot : MonoBehaviour
    {
        public Slot slot;
        private Dictionary<ushort, GameObject> _activeEffects = new Dictionary<ushort, GameObject>();

        public List<ushort> GetEffectIds()
        {
            List<ushort> effectIds = new List<ushort>();

            foreach (KeyValuePair<ushort,GameObject> valuePair in _activeEffects)
            {
                if (valuePair.Value != null)
                {
                    effectIds.Add(valuePair.Key);
                }
            }

            return effectIds;
        }

        public void AddEffect(ushort sessionId, VisualEffectData effectData)
        {
            ParticleSystem thisParticleSystem;
            
            if (_activeEffects.ContainsKey(sessionId) && _activeEffects[sessionId] != null)
            {
                // Refresh
                thisParticleSystem = _activeEffects[sessionId].GetComponent<ParticleSystem>();
                
                if (thisParticleSystem != null)
                {
                    thisParticleSystem.Stop();
                }
            }
            else
            {
                GameObject effectGo = PoolManager.Spawn(effectData.VisualEffectPrefab, transform.position, transform.rotation);
                effectGo.transform.parent = transform;
                effectGo.transform.localPosition = Vector3.zero;
                effectGo.transform.localScale = new Vector3(1,1,1);
                _activeEffects.Add(sessionId, effectGo);
                
                thisParticleSystem = effectGo.GetComponent<ParticleSystem>();

                if (thisParticleSystem != null)
                {
                    // thisParticleSystem.Clear();
                }
            }

            if (thisParticleSystem != null)
            {
                thisParticleSystem.Play();
            }
            else
            {
                Debug.LogWarning("Missing particle system!");
            }
            
        }

        public void RemoveEffect(ushort sessionId)
        {
            if (_activeEffects.ContainsKey(sessionId))
            {
                try
                {
                    if (_activeEffects[sessionId] != null)
                    {
                        PoolManager.Despawn(_activeEffects[sessionId]);
                    }
                }
                catch (NullReferenceException e) { }
                
                _activeEffects.Remove(sessionId);
            }
        }

        public void Clear()
        {
            foreach (var effect in _activeEffects)
            {
                PoolManager.Despawn(effect.Value);
            }
            
            _activeEffects.Clear();
        }

    }
}