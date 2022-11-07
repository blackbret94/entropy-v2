using TanksMP;
using UnityEngine.Serialization;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.TanksExtensions
{
    // Based on PowerupBullet.  May be smart to unify the common code at a later point
    public class PowerupBuff : Collectible
    {
        public Powerup Powerup;

        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// Check for the current buff and refills its time.
        /// </summary>
        public override bool Apply(Player p)
        {
            if (p == null)
                return false;

            float value = p.GetView().GetBuffSeconds();
            int currentIndex = p.GetView().GetBuffIndex();
            
            //do not consume buff if the player owns the new buff already
            //and the timer is at the maximum amount available
            if (value >= Powerup.MaxValue && currentIndex == Powerup.PowerupId)
                return false;

            //otherwise assign new bullet and refill ammo
            p.GetView().SetBuff(Powerup.MaxValue, Powerup.PowerupId);

            //return successful collection
            return true;
        }
    }
}