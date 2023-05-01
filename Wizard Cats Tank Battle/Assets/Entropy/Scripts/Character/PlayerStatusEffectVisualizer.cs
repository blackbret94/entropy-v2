using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.Character
{
    public class PlayerStatusEffectVisualizer : MonoBehaviour
    {
        public StatusEffectVisualizerUnit
            SpeedBoostVisualizer,
            RapidFireVisualizer,
            SpikeDamageVisualizer,
            ReflectionVisualizer;

        private void Start()
        {
            Spawn();
            ToggleAll(false);
        }

        private void Spawn()
        {
            Instantiate(SpeedBoostVisualizer.EffectPrefab, SpeedBoostVisualizer.EffectRoot.transform);
            Instantiate(RapidFireVisualizer.EffectPrefab, RapidFireVisualizer.EffectRoot.transform);
            Instantiate(SpikeDamageVisualizer.EffectPrefab, SpikeDamageVisualizer.EffectRoot.transform);
            Instantiate(ReflectionVisualizer.EffectPrefab, ReflectionVisualizer.EffectRoot.transform);
        }

        private void ToggleAll(bool enable)
        {
            SpeedBoostVisualizer.Toggle(enable);
            RapidFireVisualizer.Toggle(enable);
            ReflectionVisualizer.Toggle(enable);
            SpikeDamageVisualizer.Toggle(enable);
        }

        public void Refresh(SortedSet<string> indexedIds)
        {
            SpeedBoostVisualizer.Toggle(indexedIds);
            RapidFireVisualizer.Toggle(indexedIds);
            ReflectionVisualizer.Toggle(indexedIds);
            SpikeDamageVisualizer.Toggle(indexedIds);
        }
    }
}