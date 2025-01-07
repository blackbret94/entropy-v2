using UnityEngine;
using UnityEngine.Serialization;
using Vashta.Entropy.ScriptableObject;

namespace TanksMP
{
    /// <summary>
    /// Defines properties of a team.
    /// </summary>
    [System.Serializable]
    public class TeamInstance 
    {
        public TeamDefinition teamDefinition;
        
        /// <summary>
        /// The spawn point of a team in the scene. In case it has a BoxCollider
        /// component attached, a point within the collider bounds will be used.
        /// </summary>
        [FormerlySerializedAs("spawn")] public Transform spawnArea;
        [FormerlySerializedAs("freeClassChange")] public Transform freeClassChangeArea;
        
    }
}