using TanksMP;
using UnityEngine.Serialization;

namespace Vashta.Entropy.TanksExtensions
{
    // Based on PowerupBullet.  May be smart to unify the common code at a later point
    public class PowerupBuff : Collectible
    {
        /// <summary>
        /// Number of seconds buff lasts
        /// </summary>
        public float buffSeconds = 60;

        /// <summary>
        /// Index of the powerup, on the Player script, that should be assigned
        /// </summary>
        public int buffIndex = 1;

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
            if (value >= buffSeconds && currentIndex == buffIndex)
                return false;

            //otherwise assign new bullet and refill ammo
            p.GetView().SetBuff(buffSeconds, buffIndex);

            //return successful collection
            return true;
        }
    }
}