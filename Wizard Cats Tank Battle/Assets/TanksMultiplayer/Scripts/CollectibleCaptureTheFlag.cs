using Photon.Pun;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace TanksMP
{
    public class CollectibleCaptureTheFlag : CollectibleTeam
    {
         public StatusEffectData StatusEffectToApply;
         private Player _carriedBy;
         
         private int _defaultTeamIndex = -1;

         private void Start()
         {
             _defaultTeamIndex = teamIndex;
         }

        /// <summary>
        /// Server only: check for players colliding with the powerup.
        /// Possible collision are defined in the Physics Matrix.
        /// </summary>
        public override void OnTriggerEnter(Collider col)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            GameObject obj = col.gameObject;
            Player player = obj.GetComponent<Player>();

            //try to apply collectible to player, the result should be true
            if (Apply(player))
            {
                _carriedBy = player;
                _carriedBy.StatusEffectController.AddStatusEffect(StatusEffectToApply.Id, player);
                Colorize();
                
                if (spawner == null)
                {
                    Debug.LogError("Missing spawner connection!");
                }
                
                //clean up previous buffered RPCs so we only keep the most recent one
                PhotonNetwork.RemoveRPCs(spawner.photonView);

                //check if colliding player belongs to the same team as the item
                // if (teamIndex == player.GetView().GetTeam())
                // {
                    //player collected team item, return it to team home base
                    //we do not have to send this as buffered RPC because this is the default spawn position
                    // spawner.photonView.RPC("Return", RpcTarget.All);
                // }
                // else
                // {
                    //player picked up item from other team, send out buffered RPC for it to be remembered
                    spawner.photonView.RPC("Pickup", RpcTarget.AllBuffered, (short)player.GetView().ViewID);
                // }
            }
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// Check for the carrier and item position to decide valid pickup.
        /// </summary>
        public override bool Apply(Player p)
        {
            if (p == null)
                return false;
            
            int playerTeam = p.GetView().GetTeam();

            // Ignore if player is not on the right team
            if(!CanBeGrabbedBy(playerTeam))
                return false;
            
            // Set team index
            teamIndex = playerTeam;
            
            //do not allow collection if the item is already carried around
            //but also skip any processing if our flag is on the home base already
            if(carrierId > 0 || _carriedBy != null)
                return false;
                
            //return successful collection
            return true;
        }

        private bool CanBeGrabbedBy(int teamTryingToGrab)
        {
            return teamTryingToGrab == teamIndex || teamIndex == -1;
        }
        
        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// </summary>
        public override void OnDrop()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // remove status effect
                _carriedBy.StatusEffectController.RemoveStatusEffect(StatusEffectToApply.Id);
            }

            ResetFlag();
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// </summary>
        public override void OnReturn()
        {
            if (PhotonNetwork.IsMasterClient && _carriedBy != null)
            {
                // remove status effect
                _carriedBy.StatusEffectController.RemoveStatusEffect(StatusEffectToApply.Id);
            }

            ResetFlag();
        }

        private void ResetFlag()
        {
            _carriedBy = null;
            carrierId = -1;
            teamIndex = _defaultTeamIndex;

            Colorize();
        }
        
        private void Colorize()
        {
            if (targetRenderer != null)
            {
                if (teamIndex >= 0)
                    targetRenderer.material.color = GameManager.GetInstance().teams[teamIndex].material.color;
                else
                    targetRenderer.material.color = Color.white;
            }
        }
    }
}