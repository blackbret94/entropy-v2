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
        private float _refreshRate = .25f;

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
                StatusEffectSlot slot = StatusEffectBlocks[iconIndex];

                if (slot.GetStatusEffect() == statusEffect)
                {
                    // consider blinking or fading out
                    // slot.UpdateAnimation();
                }
                else
                {
                    slot.SetStatusEffect(statusEffect);
                }

                iconIndex++;
            }
        }
    }
}