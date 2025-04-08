/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Entropy.Scripts.Player;
using UnityEngine;
using Vashta.Entropy.Player;

namespace TanksMP
{
    /// <summary>
    /// Custom Collectible implementation for scene owned (unassigned) or team owned items.
    /// E.g. allowing for 'Rambo' pickups, Capture the Flag items etc.
    /// </summary>
	public class CollectibleTeam : Collectible
    {
        /// <summary>
        /// Team index this Collectible belongs to, or -1 if unassigned.
        /// Teams are defined in the GameManager script inspector.
        /// </summary>
        public int teamIndex = -1;

        /// <summary>
        /// Optional: Material that should be re-assigned if this Collectible is dropped or returned.
        /// </summary>
        public Material baseMaterial;

        /// <summary>
        /// Optional: Renderer on which the material should be modified depending on carrier team.
        /// </summary>
        public MeshRenderer targetRenderer;


        /// <summary>
        /// Server only: check for players colliding with the powerup.
        /// Possible collision are defined in the Physics Matrix.
        /// </summary>
        public override void OnTriggerEnter(Collider col)
        {
            GameObject obj = col.gameObject;
            PlayerController playerController = obj.GetComponent<PlayerController>();

            //try to apply collectible to player, the result should be true
            if (Apply(playerController))
            {
                if (spawner == null)
                {
                    Debug.LogError("Missing spawner connection!");
                }
                
                // TODO: Re-write this fusion-style

                //check if colliding player belongs to the same team as the item
                if (teamIndex == playerController.TeamIndex)
                {
                    //player collected team item, return it to team home base
                    //we do not have to send this as buffered RPC because this is the default spawn position
                    spawner.Return();
                }
                else
                {
                    //player picked up item from other team, send out buffered RPC for it to be remembered
                    spawner.Pickup(playerController);

                    if (playerController.IsLocal)
                    {
                        GameManager.GetInstance().ui.DropCollectiblesButton.gameObject.SetActive(true);
                    }
                }
            }
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// Check for the carrier and item position to decide valid pickup.
        /// </summary>
        public override bool Apply(PlayerController p)
        {
            //do not allow collection if the item is already carried around
            //but also skip any processing if our flag is on the home base already
            if (p == null || carrier != null ||
                teamIndex == p.TeamIndex && transform.position == spawner.transform.position)
                return false;

            //if a target renderer is set, assign team material
            Colorize(p.TeamIndex);

            //return successful collection
            return true;
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// </summary>
        public override void OnDrop()
        {
            Colorize(this.teamIndex);
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// </summary>
        public override void OnReturn()
        {
            Colorize(this.teamIndex);
        }


        //assign material based on team index passed in
        void Colorize(int teamIndex)
        {
            if (targetRenderer != null)
            {
                if (teamIndex >= 0)
                    targetRenderer.material = GameManager.GetInstance().TeamController.teams[teamIndex].teamDefinition.Material;
                else
                    targetRenderer.material = baseMaterial;
            }
        }
        
        public override void OnPickup()
        {
            // PlayerController localPlayerController = PlayerList.GetLocalPlayer();
            //
            // if (!localPlayerController)
            // {
            //     Debug.LogError("Local player not found!");
            //     return;
            // }
            //
            // GameManager.GetInstance().ui.DropCollectiblesButton.gameObject.SetActive(true);
        }
    }
}