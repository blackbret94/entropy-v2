using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.SaveLoad
{
    /// <summary>
    /// This class is used to restrict AI agents from attempting to reach enemy safezones.
    /// </summary>
    public class PathfindingZone : MonoBehaviour
    {
        public int AllowedTeamIndex;
        public Collider _collider;
        
        private static List<PathfindingZone> _listOfPathfindingZones = new List<PathfindingZone>();

        public Collider Collider => _collider;
        
        private void Start()
        {
            _collider = GetComponent<Collider>();
            _listOfPathfindingZones.Add(this);
        }

        private void OnDestroy()
        {
            _listOfPathfindingZones.Remove(this);
        }

        public static bool PointIsInRestrictedZone(Vector3 pos, int teamIndex)
        {
            foreach (PathfindingZone zone in _listOfPathfindingZones)
            {
                if (zone.AllowedTeamIndex == teamIndex)
                    continue;

                if (zone.Collider.bounds.Contains(pos))
                {
                    return true;
                }
            }

            return false;
        }
    }
}