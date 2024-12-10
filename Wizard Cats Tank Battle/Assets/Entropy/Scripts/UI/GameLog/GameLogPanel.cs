using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI.GameLog
{
    public class GameLogPanel : GamePanel
    {
        public float TimeUntilFade;
        public List<GameLogRowPanel> GameLogRowPanels;
        private LinkedList<GameLogRow> _gameLogs = new();

        private float _lastRefreshTime;
        private float _refreshRate = .25f;

        private void Update()
        {
            if (Time.time > _lastRefreshTime + _refreshRate)
            {
                RefreshList();
            }
        }
        
        public void EventPlayerJoined(string playerName)
        {
            AddEvent($"{playerName} joined");
        }

        public void EventPlayerLeft(string playerName)
        {
            AddEvent($"{playerName} left");
        }

        public void EventPlayerKilled(string playerKilledName, TeamDefinition playerKilledTeamDefinition, string playerKillerName, TeamDefinition playerKillerTeamDefinition)
        {
            string killerColor = ColorUtility.ToHtmlStringRGB(playerKillerTeamDefinition.TeamColorPrim);
            string killedColor = ColorUtility.ToHtmlStringRGB(playerKilledTeamDefinition.TeamColorPrim);
            
            AddEvent($"<color=#{killerColor}>{playerKillerName}</color> knocked out <color=#{killedColor}>{playerKilledName}</color>");
        }

        public void EventPlayerChangedTeam(string playerName, TeamDefinition playerTeamDefinition)
        {
            string color = ColorUtility.ToHtmlStringRGB(playerTeamDefinition.TeamColorPrim);
            
            AddEvent($"<color=#{color}>{playerName}</color> joined Team <color=#{color}>{playerTeamDefinition.TeamNameDisplay}</color>");
        }

        public void EventSpoonPickedUp(string playerName, TeamDefinition playerTeamDefinition)
        {
            string color = ColorUtility.ToHtmlStringRGB(playerTeamDefinition.TeamColorPrim);
            AddEvent($"<color=#{color}>Spoon</color> picked up by <color=#{color}>{playerName}</color>");
        }

        public void EventSpoonDropped(string playerName, TeamDefinition playerTeamDefinition)
        {
            string color = ColorUtility.ToHtmlStringRGB(playerTeamDefinition.TeamColorPrim);
            AddEvent($"<color=#{color}>Spoon</color> dropped by <color=#{color}>{playerName}</color>");
        }

        public void EventSpoonCaptured(string playerName, TeamDefinition playerTeamDefinition)
        {
            string color = ColorUtility.ToHtmlStringRGB(playerTeamDefinition.TeamColorPrim);
            AddEvent($"<color=#{color}>Spoon</color> captured by <color=#{color}>{playerName}</color>");
        }

        public void EventCapturePointCaptured(TeamDefinition playerTeamDefinition)
        {
            string color = ColorUtility.ToHtmlStringRGB(playerTeamDefinition.TeamColorPrim);
            AddEvent($"<color=#{color}>Point captured</color>");
        }

        public void EventCapturePointContested()
        {
            AddEvent("Point contested");
        }

        private void AddEvent(string eventString)
        {
            GameLogRow newRow = new GameLogRow(Time.time, TimeUntilFade, eventString);
            _gameLogs.AddFirst(newRow);
            
            RefreshList();
        }

        private void RefreshList()
        {
            _lastRefreshTime = Time.time;

            var node = _gameLogs.First;
            while (node != null)
            {
                var nextNode = node.Next;

                if (node.Value.ShouldDelete())
                {
                    _gameLogs.Remove(node);
                }

                node = nextNode;
            }

            node = _gameLogs.First;
            for (int i = 0; i < GameLogRowPanels.Count; i++)
            {
                GameLogRowPanel panel = GameLogRowPanels[i];
                
                if (node != null)
                {
                    GameLogRow row = node.Value;
                    panel.SetText(row);

                    if (row.IsExpired())
                    {
                        panel.FadeOut();
                    }
                    
                    node = node.Next;
                }
                else
                {
                    panel.ResetText();
                }
            }   
        }
    }
}