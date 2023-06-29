﻿/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Vashta.Entropy.ScriptableObject;

namespace TanksMP
{
    /// <summary>
    /// Custom powerup implementation for adding player shield points.
    /// </summary>
	public class PowerupShield : Collectible
    {
        public Powerup Powerup;
        
        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// Check for the current shield and adds additional shield points.
        /// </summary>
        public override bool Apply(Player p)
        {
            if (p == null)
                return false;

            //assign absolute shield points to player
            p.GetView().SetShield(p.maxShield);
            p.GetView().SetHealth(p.maxHealth);
            p.CmdShowPowerupUI(Powerup.PowerupId);
            
            //return successful collection
            return true;
        }
    }
}
