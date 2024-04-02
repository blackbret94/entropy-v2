using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Start the timer and activate the map to play the collect game
    /// </summary>
    public class VRG_5sPlay : VRG_Base
    {
        /// #IGNORE
        [Tooltip("The current round of the play")]
        [SerializeField] private int m_Round = 0;
        /// <summary>
        /// The current round of the play
        /// </summary>
        public int round { get { return this.m_Round; } set { this.m_Round = value; } }

        /// <summary>
        /// The CountdownTimer component to modify
        /// </summary>        
        [Header("The CountdownTimer component to modify")]
        [SerializeField] private VRG_5sTimer m_Timer = null;

        /// <summary>
        /// The map of the checks, X's and star
        /// </summary>
        [Tooltip("The map of the checks, X's and star")]
        [SerializeField] private VRG_5sMap m_Map = null;

        /// <summary>
        /// Objects to trigger when you win
        /// </summary>
        [Tooltip("Objects to trigger when you win")]
        [SerializeField] private GameObject[] m_YouWin = null;


        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Show the map, count and check the game flow
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // whem the timer is defined
            if (this.m_Timer != null)
            {
                // another spin
                this.m_Round++;

                // if the timer is still valid
                if (this.m_Timer.timeMax >= this.m_Timer.winTime && (this.m_Round <= this.m_Timer.winRound || this.m_Timer.winRound == 0))
                {
                    // show the checks, X and stars
                    this.m_Map.Show();

                    //.. and play the timer
                    this.m_Timer.Play();
                }
                else
                {
                    // cycle the you win objects
                    foreach (GameObject child in this.m_YouWin)
                    {
                        child.SetActive(true);
                    }
                }

            }

            // next frame
            yield return null;
        }


    }
}