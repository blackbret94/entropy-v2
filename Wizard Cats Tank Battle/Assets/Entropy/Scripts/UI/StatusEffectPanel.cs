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

        private float _sortRateS = 1f;
        private float _lastSortTime;

        private void Awake()
        {
            _lastRefreshTime = Time.time + 1f;
            _lastSortTime = Time.time + 1f;
        }
        
        private void Update()
        {
            if (Player == null)
                return;

            // Refresh effects
            if (Time.time >= _lastRefreshTime + _refreshRate)
            {
                RefreshStatusEffects();
                _lastRefreshTime = Time.time;
            }
            
            // Resort
            // if (Time.time >= _lastSortTime + _sortRateS)
            // {
                // SortSlots();

                // _lastSortTime = Time.time;
            // }
        }

        public void ForceRefresh()
        {
            RefreshStatusEffects();
            _lastRefreshTime = Time.time;
        }

        private void RefreshStatusEffects()
        {
            List<StatusEffect> statusEffects = new List<StatusEffect>(Player.StatusEffectController.StatusEffects);
            statusEffects.Sort(CompareStatusEffects);
            SortSlots();

            for(int i=0; i<StatusEffectBlocks.Count; i++)
            {
                StatusEffectSlot slot = StatusEffectBlocks[i];
                
                // Hide blocks without status effects
                if (i >= statusEffects.Count)
                {
                    // slot.ResetStatusEffect();
                    continue;
                }

                StatusEffect statusEffect = statusEffects[i];

                if (slot.GetStatusEffect() != statusEffect)
                {
                    slot.SetStatusEffect(statusEffect);
                }
            }
        }

        private void SortSlots()
        {
            StatusEffectBlocks.Sort(CompareStatusEffectSlots);

            for (int i = 0; i < StatusEffectBlocks.Count; i++)
            {
                StatusEffectBlocks[i].transform.SetSiblingIndex(i);
            }
        }

        public void ResetSlots()
        {
            foreach (var slot in StatusEffectBlocks)
            {
                slot.ResetStatusEffect();
            }
        }
        
        private int CompareStatusEffectSlots(StatusEffectSlot x, StatusEffectSlot y)
        {
            if (x.GetStatusEffect() == null && y.GetStatusEffect() == null)
                return 0;

            if (x.GetStatusEffect() == null)
                return 1;

            if (y.GetStatusEffect() == null)
                return -1;

            if (x.GetStatusEffect().ExpirationTime() < y.GetStatusEffect().ExpirationTime())
                return -1;

            return 1;
        }
        
        private int CompareStatusEffects(StatusEffect x, StatusEffect y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            if (x.ExpirationTime() < y.ExpirationTime())
                return -1;

            return 1;
        }
    }
}