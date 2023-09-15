using System;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.UI
{
    public class StatusEffectPanel : GamePanel
    {
        public List<StatusEffectSlot> StatusEffectBlocks;
        public Player Player;
        private float _lastRefreshTime;
        private float _refreshRate = .15f;

        private void Awake()
        {
            _lastRefreshTime = Time.time + 1f;
        }
        
        private void Update()
        {
            if (Player == null)
                return;

            if (Time.time < _lastRefreshTime + _refreshRate)
                return;
            
            RefreshStatusEffects();
            _lastRefreshTime = Time.time;
        }

        private void RefreshStatusEffects()
        {
            List<StatusEffect> statusEffects = Player.StatusEffectController.StatusEffects;
            int iconIndex = 0;

            foreach (var statusEffect in statusEffects)
            {
                // Return if there are more status effects than there are slots
                if (iconIndex >= StatusEffectBlocks.Count)
                    return;
                
                StatusEffectSlot slot = StatusEffectBlocks[iconIndex];

                if (slot.GetStatusEffect() != statusEffect)
                {
                    slot.SetStatusEffect(statusEffect);
                }

                iconIndex++;
            }
        }

        public void ResetSlots()
        {
            foreach (var slot in StatusEffectBlocks)
            {
                slot.ResetStatusEffect();
            }
        }
    }
}