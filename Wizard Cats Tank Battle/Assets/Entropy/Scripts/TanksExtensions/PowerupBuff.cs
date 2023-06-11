using TanksMP;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.TanksExtensions
{
    // Based on PowerupBullet.  May be smart to unify the common code at a later point
    public class PowerupBuff : Collectible
    {
        public StatusEffectData StatusEffectData;

        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// Check for the current buff and refills its time.
        /// </summary>
        public override bool Apply(Player p)
        {
            if (p == null)
                return false;

            if (StatusEffectData != null)
            {
                return ApplyStatusEffect(p);
            }

            Debug.LogError("Could not apply a status effect, was missing statusEffectData");
            return false;
        }

        private bool ApplyStatusEffect(Player p)
        {
            p.StatusEffectController.AddStatusEffect(StatusEffectData.Id, p);

            return true;
        }
    }
}