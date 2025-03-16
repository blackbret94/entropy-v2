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
            ReflectionVisualizer;

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

        public List<ushort> GetActiveVisualizations()
        {
            List<ushort> activeVisualizationIds = new List<ushort>();
            
            // iterate over slots
            foreach (StatusEffectVisualizerSlot slot in slots)
            {
                activeVisualizationIds.AddRange(slot.GetEffectIds());
            }

            return activeVisualizationIds;
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

        public void Refresh(SortedSet<ushort> activeEffectIds)
        {
            SpeedBoostVisualizer.Toggle(activeEffectIds);
            RapidFireVisualizer.Toggle(activeEffectIds);
            ReflectionVisualizer.Toggle(activeEffectIds);
            SpikeDamageVisualizer.Toggle(activeEffectIds);
            
            List<ushort> visualIds = GetActiveVisualizations();
            
            // string debug = "Active effect IDs: ";
            // foreach (ushort activeEffectId in activeEffectIds)
            // {
            //     debug += activeEffectId + ", ";
            // }
            //
            // Debug.Log(debug);
            
            foreach (ushort visualId in visualIds)
            {
                if (!activeEffectIds.Contains(visualId))
                {
                    RemoveEffect(visualId);
                }
            }
        }

        public void Clear()
        {
            Init();
            
            foreach (var slot in _visualizerBySlot)
            {
                slot.Value.Clear();
            }
        }
        
        public void AddEffect(ushort sessionId, VisualEffectData effectData)
        {
            Init();

            if (effectData == null)
                return;

            if (_visualizerBySlot.ContainsKey(effectData.slot))
            {
                _visualizerBySlot[effectData.slot].AddEffect(sessionId, effectData);
            }
        }

        public void RemoveEffect(ushort effectSessionId)
        {
            foreach (StatusEffectVisualizerSlot slot in slots)
            {
                slot.RemoveEffect(effectSessionId);
            }
        }
    }
}