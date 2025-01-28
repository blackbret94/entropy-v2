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
        private Dictionary<string, GameObject> _activeEffects = new Dictionary<string, GameObject>();

        public void AddEffect(VisualEffectData effectData)
        {
            ParticleSystem thisParticleSystem;
            
            if (_activeEffects.ContainsKey(effectData.Id) && _activeEffects[effectData.Id] != null)
            {
                // Refresh
                thisParticleSystem = _activeEffects[effectData.Id].GetComponent<ParticleSystem>();
                
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
                _activeEffects.Add(effectData.Id, effectGo);
                
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

        public void RemoveEffect(VisualEffectData effectData)
        {
            if (_activeEffects.ContainsKey(effectData.Id) && _activeEffects[effectData.Id] != null)
            {
                try
                {
                    PoolManager.Despawn(_activeEffects[effectData.Id]);
                }
                catch (NullReferenceException e) { }
                
                _activeEffects.Remove(effectData.Id);

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