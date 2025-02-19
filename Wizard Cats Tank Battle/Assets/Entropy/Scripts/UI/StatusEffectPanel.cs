using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.Player;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.UI
{
    public class StatusEffectPanel : GamePanel
    {
        [FormerlySerializedAs("StatusEffectBlocks")] public List<StatusEffectSlot> StatusEffectSlots;
        [FormerlySerializedAs("Player")] public PlayerController playerController;
        private float _lastRefreshTime;
        private float _refreshRate = .15f;
        
        private void Awake()
        {
            _lastRefreshTime = Time.time + 1f;
        }
        
        private void Update()
        {
            if (playerController == null)
                return;

            // Refresh effects
            if (Time.time >= _lastRefreshTime + _refreshRate)
            {
                RefreshStatusEffects();
                _lastRefreshTime = Time.time;
            }
        }

        public void ForceRefresh()
        {
            RefreshStatusEffects();
            _lastRefreshTime = Time.time;
        }

        private void RefreshStatusEffects()
        {
            List<StatusEffectView> statusEffects = new List<StatusEffectView>(playerController.StatusEffectController.GetStatusEffectViews());
            statusEffects.Sort(CompareStatusEffects);
            SortSlots();

            for(int i=0; i<StatusEffectSlots.Count; i++)
            {
                StatusEffectSlot slot = StatusEffectSlots[i];
    
                // Hide blocks without status effects
                if (i >= statusEffects.Count)
                {
                    slot.ResetStatusEffect();
                    continue;
                }

                StatusEffectView statusEffectView = statusEffects[i];

                if (slot.GetStatusEffect() != statusEffectView)
                {
                    slot.SetStatusEffect(statusEffectView);
                }
            }
        }

        private void SortSlots()
        {
            StatusEffectSlots.Sort(CompareStatusEffectSlots);

            for (int i = 0; i < StatusEffectSlots.Count; i++)
            {
                StatusEffectSlots[i].transform.SetSiblingIndex(i);
            }
        }

        public void ResetSlots()
        {
            foreach (var slot in StatusEffectSlots)
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

            if (x.GetStatusEffect().StatusEffect.ExpirationTime() < y.GetStatusEffect().StatusEffect.ExpirationTime())
                return -1;

            return 1;
        }
        
        private int CompareStatusEffects(StatusEffectView x, StatusEffectView y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            if (x.StatusEffect.ExpirationTime() < y.StatusEffect.ExpirationTime())
                return -1;

            return 1;
        }
    }
}