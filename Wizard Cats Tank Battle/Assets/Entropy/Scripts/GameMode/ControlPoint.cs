using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.GameMode
{
    public class ControlPoint : MonoBehaviour
    {
        // What team CONTROLS this (earns points/tick)
        public int ControlledByTeamIndex { get; set; } = -1;

        // What team is control LEANING TOWARDS (during capture)
        public int CaptureTeamIndex { get; set; } = -1;
        public ControlPointGraphics ControlPointGraphics;
        public TeamDefinition TeamDefinitionNeutral;

        private int _captureTicks;
        private int _ticksToCapture = 100;
        private HashSet<Player> _playersInBounds;

        private bool _hasInit;
        
        private void Init()
        {
            if (_hasInit)
                return;

            _playersInBounds = new HashSet<Player>();
            ControlPointGraphics.ChangeTeamColor(TeamDefinitionNeutral);
            
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
                    _ticksToCapture = Mathf.Min(_captureTicks + 1, _ticksToCapture);
                }
                // Uncapture towards 0
                else
                {
                    _ticksToCapture = Mathf.Max(_captureTicks - 1, 0);
                }
            }
            
            // Calculate who is capturing
            // Check if the state should change back to neutral
            if (_captureTicks == 0)
            {
                CaptureTeamIndex = teamIndex;
                
                ControlledByTeamIndex = -1;
                ControlPointGraphics.ChangeTeamColor(TeamDefinitionNeutral);
            }
            else
            {
                // Calculate who controls the point
                if (_captureTicks == _ticksToCapture)
                {
                    // Award the capture
                    if (ControlledByTeamIndex != teamIndex)
                    {
                        ControlledByTeamIndex = teamIndex;
                        Team team = GameManager.GetInstance().GetTeamByIndex(teamIndex);
                        ControlPointGraphics.ChangeTeamColor(team.teamDefinition);
                        // Need to relay changes - RPC?
                    }
                }
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