using UnityEngine;

using System.Collections;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Simple and efective: Spawn a random prefab from the prefabs list
    /// </summary>
    public class VRG_SpawnPrefab : VRG_Base
    {
        [Header("From: Spawning")]
        /// <summary>
        /// The amount of prefabs to spawn
        /// </summary>
        [Tooltip("The amount of prefabs to spawn")]
        [SerializeField] private int m_HowMany = 1;

        /// <summary>
        /// The random Prefab to Spawn
        /// </summary>
        [Tooltip("The random Prefab to Spawn list")]
        [SerializeField] private GameObject[] m_Prefabs = null;


        // Enumerator proxy, it is activated OnEnable
        protected override IEnumerator Do()
        {
            // if it is defined
            if (this.m_Prefabs.Length > 0 && this.m_HowMany > 0)
            {
                // spawn this.m_HowMany prefabs
                for (int i = 0; i < this.m_HowMany; i++)
                {
                    // from the random Array of this.m_Prefabs
                    Object.Instantiate(this.m_Prefabs[Random.Range(0, this.m_Prefabs.Length)]);
                }
            }

            // can't spawn anything
            else
            {
                this.Logs("NO Prefabs to spawn, please add at least one", ENUM_Verbose.WARNING);
            }

            // finish the next frame
            yield return null;
        }


    }
}