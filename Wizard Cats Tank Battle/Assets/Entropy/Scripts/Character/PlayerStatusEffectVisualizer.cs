using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.ScriptableObject;

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

        [FormerlySerializedAs("Slots")] public List<StatusEffectVisualizerSlot> slots;
        private Dictionary<Slot, StatusEffectVisualizerSlot> _visualizerBySlot;

        private bool _hasInit;

        private void Start()
        {
            Spawn();
            ToggleAll(false);
            
            Init();
        }

        private void Init()
        {
            if (_hasInit)
                return;
            
            PopulateDictionary();

            _hasInit = true;
        }

        private void PopulateDictionary()
        {
            _visualizerBySlot = new Dictionary<Slot, StatusEffectVisualizerSlot>();
            
            foreach (StatusEffectVisualizerSlot slot in slots)
            {
                _visualizerBySlot.Add(slot.slot, slot);
            }
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

        public void Clear()
        {
            foreach (var slot in _visualizerBySlot)
            {
                slot.Value.Clear();
            }
        }
        
        public void AddEffect(VisualEffect effect)
        {
            Init();

            if (effect == null)
                return;

            if (_visualizerBySlot.ContainsKey(effect.slot))
            {
                _visualizerBySlot[effect.slot].AddEffect(effect);
            }
            else
            {
                Debug.Log("No slot for: " + effect.slot);
            }
        }

        public void RemoveEffect(VisualEffect effect)
        {
            Init();

            if (effect == null)
                return;

            if (_visualizerBySlot.ContainsKey(effect.slot))
            {
                _visualizerBySlot[effect.slot].RemoveEffect(effect);
            }
            else
            {
                Debug.Log("No slot for: " + effect.slot);
            }
        }
    }
}