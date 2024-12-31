using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.TanksExtensions
{
    public class PowerupRandom : Collectible
    {
        public List<StatusEffectData> PossibleStatusEffects;
        public StatusEffectDirectory StatusEffectDirectory;

        public override bool Apply(Player p)
        {
            if (p == null)
                return false;

            if (PossibleStatusEffects.Count == 0)
            {
                Debug.LogError("Random powerup does not have any possible options!");
            }
            
            StatusEffectData statusEffectData = ChooseStatusEffect();

            if (statusEffectData != null)
            {
                return ApplyStatusEffect(p, statusEffectData);
            }

            Debug.LogError("Could not apply a status effect, was missing statusEffectData");
            return false;
        }

        private StatusEffectData ChooseStatusEffect()
        {
            int index = Random.Range(0, PossibleStatusEffects.Count);
            return PossibleStatusEffects[index];
        }

        private bool ApplyStatusEffect(Player p, StatusEffectData statusEffectData)
        {
            int statusEffectSessionId = StatusEffectDirectory.GetSessionId(statusEffectData);

            if (statusEffectSessionId <= 0)
            {
                Debug.LogError("Could not find status effect with ID: " + statusEffectData.Id);
                return false;
            }
            
            p.PowerupId = statusEffectSessionId;
            p.ShowPowerupIcon(statusEffectSessionId);

            return true;
        }
    }
}