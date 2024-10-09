using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class ControlPoint : MonoBehaviourPunCallbacks
    {
        // What team CONTROLS this (earns points/tick)
        public int ControlledByTeamIndex { get; private set; } = -1; // SYNCED

        // What team is control LEANING TOWARDS (during capture)
        public int CaptureTeamIndex { get; private set; } = -1; // SYNCED
        public ControlPointGraphics ControlPointGraphics;
        public TeamDefinition TeamDefinitionNeutral;

        // How many ticks the point currently has towards capture
        private int _captureTicks = 0; // SYNCED
        private int _ticksToCapture = 5;
        private HashSet<Player> _playersInBounds;

        private bool _hasInit;
        
        private void Init()
        {
            if (_hasInit)
                return;

            _playersInBounds = new HashSet<Player>();
            ControlPointGraphics.ChangeTeamColorControl(TeamDefinitionNeutral);
            
            _hasInit = true;
        }
        
        private void Start()
        {
            Init();
        }

        public void OneTickCapture()
        {
            RecalculateOwnership();
        }

        private void RecalculateOwnership()
        {
            Init();
            
            // ignore recalculation if no players are in bounds
            if (_playersInBounds.Count == 0)
            {
                return;
            }
            
            // iterate over list of players.  If only ONE team is in control, set them to be the capturing team.
            // if multiple teams are present, put it into a neutral capture state
            int teamIndex = -1;

            foreach (Player player in _playersInBounds)
            {
                if (player.IsAlive)
                {
                    if (teamIndex == -1)
                        teamIndex = player.GetTeam();
                    else
                    {
                        // if player is on a different team stop capturing
                        if(teamIndex != player.GetTeam())
                            teamIndex = -1;
                        
                        break;
                    }
                }
            }

            // Alter the state of capture ticks
            if (teamIndex != -1)
            {
                // Continue capturing
                if (teamIndex == CaptureTeamIndex)
                {
                    short numberOfTicks = (short)Mathf.Min(_captureTicks + 1, _ticksToCapture);
                    CmdSetCaptureTicks(numberOfTicks);
                }
                // Uncapture towards 0
                else
                {
                    short numberOfTicks = (short)Mathf.Max(_captureTicks - 1, 0);
                    CmdSetCaptureTicks(numberOfTicks);
                }
            }
            
            ControlPointGraphics.SetFlagPosition((float)_captureTicks/_ticksToCapture);
            
            // Calculate who is capturing
            // Check if the state should change back to neutral
            if (_captureTicks == 0)
            {
                CmdSetCaptureTeamIndex((short)teamIndex);
                CmdSetControlledByTeamIndex(-1);
            }
            else
            {
                // Calculate who controls the point
                if (_captureTicks == _ticksToCapture)
                {
                    // Award the capture
                    if (ControlledByTeamIndex != teamIndex)
                    {
                        CmdSetControlledByTeamIndex((short)teamIndex);
                    }
                }
            }
        }

        protected void RewardPointForHolding()
        {
            
        }
        
        // RPC
        // ControlledByTeamIndex
        protected void CmdSetControlledByTeamIndex(short teamIndex)
        {
            // Server only
            if (!PhotonNetwork.IsMasterClient || teamIndex == ControlledByTeamIndex)
                return;
            
            this.photonView.RPC("RpcSetControlledByTeamIndex", RpcTarget.All, teamIndex);
        }
        
        [PunRPC]
        protected void RpcSetControlledByTeamIndex(short teamIndex)
        {
            short oldIndex = (short)ControlledByTeamIndex;

            // Ignore if this is already the set team
            if (oldIndex == teamIndex)
                return;
            
            ControlledByTeamIndex = teamIndex;
            
            Team team = GameManager.GetInstance().GetTeamByIndex(teamIndex);

            if (team != null)
            {
                ControlPointGraphics.ChangeTeamColorControl(team.teamDefinition);
                ControlPointGraphics.SetFlagPosition(1);
            }
            else
            {
                Debug.LogError("Could not find team with ID: " + teamIndex);
            }

            if (teamIndex == -1)
            {
                // Color neutral
                ControlPointGraphics.ChangeTeamColorControl(TeamDefinitionNeutral);
            }
        }
        
        // CaptureTicks
        protected void CmdSetCaptureTicks(short numberOfTicks)
        {
            // Server only
            if (!PhotonNetwork.IsMasterClient || numberOfTicks == _captureTicks)
                return;
            
            this.photonView.RPC("RpcSetCaptureTicks", RpcTarget.All, numberOfTicks);
        }

        [PunRPC]
        protected void RpcSetCaptureTicks(short numberOfTicks)
        {
            short oldTicks = (short)_captureTicks;

            // Ignore if this is already the set value
            if (oldTicks == numberOfTicks)
                return;
            
            _captureTicks = numberOfTicks;
        }
        
        // CaptureTeamIndex
        protected void CmdSetCaptureTeamIndex(short teamIndex)
        {
            // Server only
            if (!PhotonNetwork.IsMasterClient || teamIndex == CaptureTeamIndex)
                return;
            
            this.photonView.RPC("RpcSetCaptureTeamIndex", RpcTarget.All, teamIndex);
        }

        [PunRPC]
        protected void RpcSetCaptureTeamIndex(short teamIndex)
        {
            short oldTeam = (short)CaptureTeamIndex;

            // Ignore if this is already the set team
            if (oldTeam == teamIndex)
                return;
            
            CaptureTeamIndex = teamIndex;
            
            Team team = GameManager.GetInstance().GetTeamByIndex(CaptureTeamIndex);

            if (team != null)
            {
                ControlPointGraphics.ChangeTeamColorCapturing(team.teamDefinition);
            }
            else
            {
                Debug.LogError("Could not find team with ID: " + teamIndex);
            }
        }

        // SERVER ONLY
        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            Init();
            
            Player player = other.GetComponent<Player>();

            if (!player)
                return;
            
            _playersInBounds.Add(player);
        }

        // SERVER ONLY
        private void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            Init();
            
            Player player = other.GetComponent<Player>();

            if (!player)
                return;

            _playersInBounds.Remove(player);
        }
    }
}