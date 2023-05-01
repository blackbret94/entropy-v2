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
            
            foreach (var block in StatusEffectBlocks)
            {
                block.ClosePanel();
            }
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
            for (int i = 0; i < StatusEffectBlocks.Count; i++)
            {
                StatusEffectSlot slot = StatusEffectBlocks[i];

                if (i < statusEffects.Count)
                {
                    StatusEffect statusEffect = statusEffects[i];
                    
                    slot.OpenPanel();
                    slot.SetStatusEffect(statusEffect);
                }
                else
                {
                    slot.ClosePanel();
                }
            }
        }
    }
}