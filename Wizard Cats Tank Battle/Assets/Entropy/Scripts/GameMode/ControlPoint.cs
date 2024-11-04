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

        public AudioClip CaptureUpAudioClip;
        public AudioClip CaptureDownAudioClip;
        public AudioClip PointCaptured;
        public AudioClip PointLost;

        // How many ticks the point currently has towards capture
        private int _captureTicks = 0; // SYNCED
        private int _ticksToCapture = 5;
        private List<Player> _playersInBounds;

        private bool _hasInit;
        private bool _wasRecentlyCaptured; // Use to determine if Lost sfx should be played
        
        private void Init()
        {
            if (_hasInit)
                return;

            _playersInBounds = new List<Player>();
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
            
            CleanList();
            
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
                int lastCaptureTicks = _captureTicks;
                
                // Continue capturing
                if (teamIndex == CaptureTeamIndex)
                {
                    short numberOfTicks = (short)Mathf.Min(_captureTicks + 1, _ticksToCapture);
                    CmdSetCaptureTicks(numberOfTicks);

                    if (numberOfTicks != lastCaptureTicks)
                    {
                        AudioManager.Play3D(CaptureUpAudioClip, transform.position);
                    }
                }
                // Uncapture towards 0
                else
                {
                    short numberOfTicks = (short)Mathf.Max(_captureTicks - 1, 0);
                    CmdSetCaptureTicks(numberOfTicks);

                    if (numberOfTicks != lastCaptureTicks)
                    {
                        AudioManager.Play3D(CaptureDownAudioClip, transform.position);
                    }
                }
            }

            float flagPosition = Mathf.Abs(_captureTicks) / (float)_ticksToCapture;
            ControlPointGraphics.SetFlagPosition(flagPosition);
            
            // Calculate who is capturing
            // Check if the state should change back to neutral
            if (_captureTicks == 0)
            {
                // Set to neutral
                CmdSetCaptureTeamIndex((short)teamIndex);
                CmdSetControlledByTeamIndex(-1);

                if (_wasRecentlyCaptured)
                {
                    AudioManager.Play3D(PointLost, transform.position);
                    _wasRecentlyCaptured = false;
                }
            }
            else
            {
                // Calculate who controls the point
                if (_captureTicks == _ticksToCapture)
                {
                    // Award the capture
                    if (teamIndex != -1 && ControlledByTeamIndex != teamIndex)
                    {
                        CmdSetControlledByTeamIndex((short)teamIndex);
                        AudioManager.Play3D(PointCaptured, transform.position);
                        _wasRecentlyCaptured = true;
                        
                        // award points
                        AwardPointsToPlayersOnCapture();
                    }
                }
            }
        }
        
        protected void AwardPointsToPlayersOnCapture()
        {
            foreach (Player player in _playersInBounds)
            {
                if (player == null)
                    continue;
                
                player.CmdRewardForControlPointCapture();
            }
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

        private void OnTriggerEnter(Collider other)
        {
            Init();
            
            Player player = other.GetComponent<Player>();

            if (!player)
                return;
            
            _playersInBounds.Add(player);
        }

        private void OnTriggerExit(Collider other)
        {
            Init();
            
            Player player = other.GetComponent<Player>();

            if (!player)
                return;

            _playersInBounds.Remove(player);
        }

        private void CleanList()
        {
            List<Player> playersInBoundsCopy = new List<Player>(_playersInBounds);
            
            foreach (Player player in playersInBoundsCopy)
            {
                if (player == null)
                {
                } else if (!player.IsAlive)
                {
                    // Remove player
                    _playersInBounds.Remove(player);
                }
            }
        }
    }
}