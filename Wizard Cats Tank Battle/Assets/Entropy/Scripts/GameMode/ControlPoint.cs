using System.Collections.Generic;
using Fusion;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
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
        private sbyte CaptureTicks { get; set; }= 0;
        private int _ticksToCapture = 5;
        private List<PlayerController> _playersInBounds;
        private GameManager _gameManager;

        private bool _hasInit;
        private bool _wasRecentlyCaptured; // Use to determine if Lost sfx should be played
        private int _lastTickTeam = -1;
        
        private void Init()
        {
            if (_hasInit)
                return;

            _playersInBounds = new List<PlayerController>();
            ControlPointGraphics.ChangeTeamColorControl(TeamDefinitionNeutral);
            _gameManager = GameManager.GetInstance();
            
            _hasInit = true;
        }
        
        private void Start()
        {
            Init();
        }

        public override void Spawned()
        {
            base.Spawned();
            
            OnCaptureTeamIndexChanged();
            OnControlledByTeamIndexChanged();
            OnCaptureTicksChanged();
        }

        public void OneTickCapture()
        {
            RecalculateOwnership();
        }

        private void RecalculateOwnership()
        {
            Init();
            
            CleanList();

            _lastTickTeam = -1;
            
            // ignore recalculation if no players are in bounds
            if (_playersInBounds.Count == 0)
            {
                return;
            }
            
            // iterate over list of players.  If only ONE team is in control, set them to be the capturing team.
            // if multiple teams are present, put it into a neutral capture state

            foreach (PlayerController player in _playersInBounds)
            {
                if (player.IsAlive)
                {
                    if (_lastTickTeam == -1)
                        _lastTickTeam = player.TeamIndex;
                    else
                    {
                        // if player is on a different team stop capturing
                        if(_lastTickTeam != player.TeamIndex)
                            _lastTickTeam = -1;
                        
                        break;
                    }
                }
            }

            // Alter the state of capture ticks
            if (_lastTickTeam != -1)
            {
                // Continue capturing
                if (_lastTickTeam == CaptureTeamIndex)
                {
                    CaptureTicks = (sbyte)Mathf.Min(CaptureTicks + 1, _ticksToCapture);
                }
                // Uncapture towards 0
                else
                {
                    CaptureTicks = (sbyte)Mathf.Max(CaptureTicks - 1, 0);
                }
            }

            float flagPosition = Mathf.Abs(CaptureTicks) / (float)_ticksToCapture;
            ControlPointGraphics.SetFlagPosition(flagPosition);
            
            // Calculate who is capturing
            // Check if the state should change back to neutral
            if (CaptureTicks == 0)
            {
                // Set to neutral
                CaptureTeamIndex = (sbyte)_lastTickTeam;
                ControlledByTeamIndex = -1;
            }
            else
            {
                // Calculate who controls the point
                if (CaptureTicks == _ticksToCapture)
                {
                    // Award the capture
                    if (_lastTickTeam != -1 && ControlledByTeamIndex != _lastTickTeam)
                    {
                        ControlledByTeamIndex = (sbyte)_lastTickTeam;
                        
                        // award points
                        AwardPointsToPlayersOnCapture();
                    }
                }
            }
        }
        
        protected void AwardPointsToPlayersOnCapture()
        {
            foreach (PlayerController player in _playersInBounds)
            {
                if (player == null)
                    continue;
                
                player.RewardForControlPointCapture();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Init();
            
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (!playerController)
                return;
            
            _playersInBounds.Add(playerController);
        }

        private void OnTriggerExit(Collider other)
        {
            Init();
            
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (!playerController)
                return;

            _playersInBounds.Remove(playerController);
        }

        private void CleanList()
        {
            List<PlayerController> playersInBoundsCopy = new List<PlayerController>(_playersInBounds);
            
            foreach (PlayerController player in playersInBoundsCopy)
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
            TeamInstance teamInstance = GameManager.GetInstance().TeamController.GetTeamByIndex(CaptureTeamIndex);
            
            if (teamInstance != null)
            {
                ControlPointGraphics.ChangeTeamColorCapturing(teamInstance.teamDefinition);
            }
            else
            {
                Debug.LogError("Could not find team with ID: " + CaptureTeamIndex);
            }
        }

        private void OnControlledByTeamIndexChanged()
        {
            if (ControlledByTeamIndex == -1)
            {
                // Color neutral
                ControlPointGraphics.ChangeTeamColorControl(TeamDefinitionNeutral);
                
                if (_wasRecentlyCaptured)
                {
                    AudioManager.Play3D(PointLost, transform.position);
                    _gameManager.ui.GameLogPanel.EventCapturePointContested();
                    _wasRecentlyCaptured = false;
                }
            }
            else
            {
                TeamInstance teamInstance = GameManager.GetInstance().TeamController.GetTeamByIndex(ControlledByTeamIndex);

                // Color for team
                ControlPointGraphics.ChangeTeamColorControl(teamInstance.teamDefinition);
                
                AudioManager.Play3D(PointCaptured, transform.position);
                _wasRecentlyCaptured = true;
                
                // notify
                _gameManager.ui.GameLogPanel.EventCapturePointCaptured(teamInstance.teamDefinition);
            }
        }
        
        private void OnCaptureTicksChanged()
        {
            if (_lastTickTeam == CaptureTeamIndex)
            {
                AudioManager.Play3D(CaptureUpAudioClip, transform.position);
            }
            else
            {
                AudioManager.Play3D(CaptureDownAudioClip, transform.position);
            }

            float flagPosition = Mathf.Abs(CaptureTicks) / (float)_ticksToCapture;
            ControlPointGraphics.SetFlagPosition(flagPosition);
        }
    }
}