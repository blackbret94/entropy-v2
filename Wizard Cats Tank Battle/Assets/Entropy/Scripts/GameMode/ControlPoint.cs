using System.Collections.Generic;
using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class ControlPoint : NetworkBehaviour
    {
        // What team CONTROLS this (earns points/tick)
        [Networked, OnChangedRender(nameof(OnControlledByTeamIndexChanged))]
        public sbyte ControlledByTeamIndex { get; private set; } = -1;

        // What team is control LEANING TOWARDS (during capture)
        [Networked, OnChangedRender(nameof(OnCaptureTeamIndexChanged))]
        public sbyte CaptureTeamIndex { get; private set; } = -1;
        public ControlPointGraphics ControlPointGraphics;
        public TeamDefinition TeamDefinitionNeutral;

        public AudioClip CaptureUpAudioClip;
        public AudioClip CaptureDownAudioClip;
        public AudioClip PointCaptured;
        public AudioClip PointLost;

        // How many ticks the point currently has towards capture
        [Networked, OnChangedRender(nameof(OnCaptureTicksChanged))]
        private sbyte _captureTicks { get; set; }= 0;
        private int _ticksToCapture = 5;
        private List<Player> _playersInBounds;
        private GameManager _gameManager;

        private bool _hasInit;
        private bool _wasRecentlyCaptured; // Use to determine if Lost sfx should be played
        
        private void Init()
        {
            if (_hasInit)
                return;

            _playersInBounds = new List<Player>();
            ControlPointGraphics.ChangeTeamColorControl(TeamDefinitionNeutral);
            _gameManager = GameManager.GetInstance();
            
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
                        teamIndex = player.TeamIndex;
                    else
                    {
                        // if player is on a different team stop capturing
                        if(teamIndex != player.TeamIndex)
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
                    _captureTicks = (sbyte)Mathf.Min(_captureTicks + 1, _ticksToCapture);

                    if (_captureTicks != lastCaptureTicks)
                    {
                        AudioManager.Play3D(CaptureUpAudioClip, transform.position);
                    }
                }
                // Uncapture towards 0
                else
                {
                    _captureTicks = (sbyte)Mathf.Max(_captureTicks - 1, 0);

                    if (_captureTicks != lastCaptureTicks)
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
                CaptureTeamIndex = (sbyte)teamIndex;
                ControlledByTeamIndex = -1;

                if (_wasRecentlyCaptured)
                {
                    AudioManager.Play3D(PointLost, transform.position);
                    
                    _gameManager.ui.GameLogPanel.EventCapturePointContested();
                    
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
                        ControlledByTeamIndex = (sbyte)teamIndex;
                        AudioManager.Play3D(PointCaptured, transform.position);
                        _wasRecentlyCaptured = true;
                        
                        // award points
                        AwardPointsToPlayersOnCapture();
                        
                        // notify
                        Team team = GameManager.GetInstance().TeamController.GetTeamByIndex(teamIndex);
                        _gameManager.ui.GameLogPanel.EventCapturePointCaptured(team.teamDefinition);
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
                
                player.RewardForControlPointCapture();
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
        
        // --------------------------------
        // Synced property render function
        // --------------------------------
        private void OnCaptureTeamIndexChanged()
        {
            Team team = GameManager.GetInstance().TeamController.GetTeamByIndex(CaptureTeamIndex);

            if (team != null)
            {
                ControlPointGraphics.ChangeTeamColorCapturing(team.teamDefinition);
            }
            else
            {
                Debug.LogError("Could not find team with ID: " + CaptureTeamIndex);
            }
        }

        private void OnControlledByTeamIndexChanged()
        {
            Team team = GameManager.GetInstance().TeamController.GetTeamByIndex(ControlledByTeamIndex);

            if (team != null)
            {
                ControlPointGraphics.ChangeTeamColorControl(team.teamDefinition);
            }
            else
            {
                Debug.LogError("Could not find team with ID: " + ControlledByTeamIndex);
            }

            if (ControlledByTeamIndex == -1)
            {
                // Color neutral
                ControlPointGraphics.ChangeTeamColorControl(TeamDefinitionNeutral);
            }
        }
        
        private void OnCaptureTicksChanged()
        {
            
        }
    }
}