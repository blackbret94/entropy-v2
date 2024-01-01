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

        public void AddEffect(VisualEffect effect)
        {
            ParticleSystem thisParticleSystem;
            
            if (_activeEffects.ContainsKey(effect.Id) && _activeEffects[effect.Id] != null)
            {
                // Refresh
                thisParticleSystem = _activeEffects[effect.Id].GetComponent<ParticleSystem>();
                
                if (thisParticleSystem != null)
                {
                    thisParticleSystem.Stop();
                }
            }
            else
            {
                GameObject effectGo = PoolManager.Spawn(effect.VisualEffectPrefab, transform.position, transform.rotation);
                effectGo.transform.parent = transform;
                effectGo.transform.localPosition = Vector3.zero;
                effectGo.transform.localScale = new Vector3(1,1,1);
                _activeEffects.Add(effect.Id, effectGo);
                
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

        public void RemoveEffect(VisualEffect effect)
        {
            if (_activeEffects.ContainsKey(effect.Id))
            {
                PoolManager.Despawn(_activeEffects[effect.Id]);
                _activeEffects.Remove(effect.Id);
            }
        }

        public void Clear()
        {
            foreach (var effect in _activeEffects)
            {
                PoolManager.Despawn(effect.Value);
            }
        }

    }
}