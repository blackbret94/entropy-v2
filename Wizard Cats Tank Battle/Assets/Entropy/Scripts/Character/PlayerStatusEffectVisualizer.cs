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
            ReflectionVisualizer,
            
            HealingVisualizer,
            BurningVisualizer,
            SunderedVisualizer,
            HardenedVisualizer;

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
            
            Instantiate(HealingVisualizer.EffectPrefab, HealingVisualizer.EffectRoot.transform);
            Instantiate(BurningVisualizer.EffectPrefab, BurningVisualizer.EffectRoot.transform);
            Instantiate(SunderedVisualizer.EffectPrefab, SunderedVisualizer.EffectRoot.transform);
            Instantiate(HardenedVisualizer.EffectPrefab, HardenedVisualizer.EffectRoot.transform);
        }

        private void ToggleAll(bool enable)
        {
            SpeedBoostVisualizer.Toggle(enable);
            RapidFireVisualizer.Toggle(enable);
            ReflectionVisualizer.Toggle(enable);
            SpikeDamageVisualizer.Toggle(enable);
            
            HealingVisualizer.Toggle(enable);
            BurningVisualizer.Toggle(enable);
            SunderedVisualizer.Toggle(enable);
            HardenedVisualizer.Toggle(enable);
        }

        public void Refresh(SortedSet<string> indexedIds)
        {
            SpeedBoostVisualizer.Toggle(indexedIds);
            RapidFireVisualizer.Toggle(indexedIds);
            ReflectionVisualizer.Toggle(indexedIds);
            SpikeDamageVisualizer.Toggle(indexedIds);
            
            HealingVisualizer.Toggle(indexedIds);
            BurningVisualizer.Toggle(indexedIds);
            SunderedVisualizer.Toggle(indexedIds);
            HardenedVisualizer.Toggle(indexedIds);
        }
    }
}