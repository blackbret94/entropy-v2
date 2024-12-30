using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Vashta.Entropy.StatusEffects;
using Vashta.Entropy.UI.Minimap;

namespace TanksMP
{
    public class CollectibleCaptureTheFlag : CollectibleTeam
    {
         public StatusEffectData StatusEffectToApply;
         private Player _carriedBy;

         public Player CarriedBy => _carriedBy;
         
         private int _defaultTeamIndex = -1;
         private MinimapEntityController _entityController;
         
         private static List<CollectibleCaptureTheFlag> _allFlags = new();
         public static List<CollectibleCaptureTheFlag> GetAllFlags() => _allFlags;

         private void Start()
         {
             _defaultTeamIndex = teamIndex;
             _allFlags.Add(this);
             _entityController = GetComponent<MinimapEntityController>();
         }

         private void OnDestroy()
         {
             _allFlags.Remove(this);
         }

         /// <summary>
        /// Server only: check for players colliding with the powerup.
        /// Possible collision are defined in the Physics Matrix.
        /// </summary>
        public override void OnTriggerEnter(Collider col)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            if (_carriedBy != null)
                return;

            GameObject obj = col.gameObject;
            Player player = obj.GetComponent<Player>();

            if (player == null) return;

            //try to apply collectible to player, the result should be true
            if (Apply(player))
            {
                // Attach
                _carriedBy = player;
                _carriedBy.StatusEffectController.AddStatusEffect(StatusEffectToApply.Id, player);
                Colorize();
                
                if (spawner == null)
                {
                    Debug.LogError("Missing spawner connection!");
                }
                
                //clean up previous buffered RPCs so we only keep the most recent one
                PhotonNetwork.RemoveRPCs(spawner.photonView);
                
                //player picked up item from other team, send out buffered RPC for it to be remembered
                spawner.photonView.RPC("Pickup", RpcTarget.AllBuffered, (short)player.GetView().ViewID);
            }
            else
            {
                // Should the flag reset?
                // Do not reset a neutral flag
                if (teamIndex == -1) return;
                
                int playerTeam = player.GetView().GetTeam();
                
                if (playerTeam != teamIndex)
                {
                    spawner.photonView.RPC("Return", RpcTarget.All);
                }
            }
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// Check for the carrier and item position to decide valid pickup.
        /// </summary>
        public override bool Apply(Player p)
        {
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
            
            // notify
            GameManager.GetInstance().ui.GameLogPanel.EventSpoonDropped(_carriedBy.GetName(), _carriedBy.GetTeamDefinition());

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

            if (_carriedBy != null)
            {
                // Notify
                GameManager.GetInstance().ui.GameLogPanel.EventSpoonCaptured(_carriedBy.GetName(), _carriedBy.GetTeamDefinition());
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
                    targetRenderer.material.color = GameManager.GetInstance().TeamController.teams[teamIndex].material.color;
                else
                    targetRenderer.material.color = Color.white;
            }
        }

        private void OnEnable()
        {
            if (_entityController)
            {
                _entityController.ShowEntity();
            }
        }

        private void OnDisable()
        {
            if (_entityController)
            {
                _entityController.HideEntity();
            }
        }
    }
}