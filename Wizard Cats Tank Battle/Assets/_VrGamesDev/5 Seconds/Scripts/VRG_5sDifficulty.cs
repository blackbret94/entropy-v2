using System.Collections;

using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// The class that has all the variables to set the difficulty of the 5second game
    /// </summary>
    [System.Serializable]
    public class VRG_5sDifficultyConfiguration
    {
        /// <summary>
        /// The time limit to win the game, by default is 1.5 seconds
        /// </summary>
        [Tooltip("The time limit to win the game, by default is 1.5 seconds")]
        [Range(1.5f, 0.5f)]
        [SerializeField] public float winTime = 1.5f;

        /// <summary>
        /// How many rounds are needed to win the game, by default is 20
        /// </summary>
        [Tooltip("How many rounds are needed to win the game, by default is 20")]
        [Range(0, 30)]
        [SerializeField] public int winRound = 20;

        /// <summary>
        /// The probability to show a Star, by default is 4
        /// </summary>
        [Tooltip("The probability to show a Star, by default is 4")]
        [Range(4, 0)]
        [SerializeField] public int starChance = 4;

        /// <summary>
        /// The time in seconds to win when you get a star, by default is 1
        /// </summary>
        [Tooltip("The time in seconds to win when you get a star, by default is 1")]
        [Range(1.0f, 0.0f)]
        [SerializeField] public float timeBonus = 1;

        /// <summary>
        /// The Maximum bonus available, by default is 0.25 seconds
        /// </summary>
        [Tooltip("The Maximum bonus available, by default is 0.25 seconds")]
        [Range(0.25f, 0.0f)]
        [SerializeField] public float timeMaxBonus = 0.25f;

        /// <summary>
        /// How many seconds you lose per round, by default is -0.10 seconds
        /// </summary>
        [Tooltip("How many seconds you lose per round, by default is -0.10 seconds")]
        [Range(-0.10f, -0.50f)]
        [SerializeField] public float timeLessPerRound = -0.10f;

        /// <summary>
        /// The duration of the game... 5 seconds game, has 5 seconds by default. Duh!
        /// </summary>
        [Tooltip("The duration of the game... 5 seconds game, has 5 seconds by default. Duh!")]
        [Range(5.0f, 0.5f)]
        [SerializeField] public float duration = 5.0f;

        /// <summary>
        /// Boolean: change color or not, by default 0 (No)
        /// </summary>
        [Tooltip("Boolean: change color or not, by default 0 (No)")]
        [Range(0, 1)]
        [SerializeField] public int colorChange = 0;

        /// <summary>
        /// When you get a Star, the bonus multiplier when the game is over, by default is 1
        /// </summary>
        [Tooltip("When you get a Star, the bonus multiplier when the game is over, by default is 1")]
        [Range(1.0f, 5.0f)]
        [SerializeField] public float starScore = 1.0f;

        /// <summary>
        /// Check mark bonus score when the game is over, by default is 1
        /// </summary>
        [Tooltip("Check mark bonus score when the game is over, by default is 1")]
        [Range(1.0f, 10.0f)]
        [SerializeField] public float bonusScore = 1.0f;
    }

    /// <summary>
    /// Adjust the game difficulty and bonuses 
    /// </summary>
    public class VRG_5sDifficulty : VRG_Base
    {
        /// <summary>
        /// The lowest difficulty, Sleeper (1)
        /// </summary>
        [Header("From: Difficulty")]
        [Tooltip("The lowest difficulty, Sleeper (1)")]
        [SerializeField] private VRG_5sDifficultyConfiguration m_Sleeper = null;

        /// <summary>
        /// The normal difficulty, Aware (2)
        /// </summary>
        [Tooltip("The normal difficulty, Aware (2)")]
        [SerializeField] private VRG_5sDifficultyConfiguration m_Aware = null;

        /// <summary>
        /// The Hard difficulty, Enlighten (3)
        /// </summary>
        [Tooltip("The Hard difficulty, Enlighten (3)")]
        [SerializeField] private VRG_5sDifficultyConfiguration m_Enlighten = null;

        /// <summary>
        /// The Hardest difficulty, Awake (4)
        /// </summary>
        [Tooltip("The Hardest difficulty, Awake (4)")]
        [SerializeField] private VRG_5sDifficultyConfiguration m_Awaken = null;



        [Header("From: Components")]

        /// <summary>
        /// From Components: The timer to update (clock backward)
        /// </summary>
        [Tooltip("From Components: The timer to update (clock backward)")]
        [SerializeField] private VRG_5sTimer m_5sTimer = null;

        /// <summary>
        /// From Components: The 3x3 map, where the game happens
        /// </summary>
        [Tooltip("From Components: The 3x3 map, where the game happens")]
        [SerializeField] private VRG_5sMap m_5sMap = null;

        /// <summary>
        /// From Components: The add module to custom
        /// </summary>
        [Tooltip("From Components: The add module to custom")]
        [SerializeField] private VRG_5sTimerAdd m_5sTimerAdd = null;

        /// <summary>
        /// From Components: The max timer, you can get above this clock time
        /// </summary>
        [Tooltip("From Components: The max timer, you can get above this clock time")]
        [SerializeField] private VRG_5sTimerMax m_5sTimerMax = null;

        /// <summary>
        /// From Components: The UI minus dancing 
        /// </summary>
        [Tooltip("From Components: The UI minus dancing")]
        [SerializeField] private VRG_5sTimerMax m_5sTimerLess = null;

        /// <summary>
        /// From Components: The score UI 
        /// </summary>
        [Tooltip("From Components: The score UI ")]
        [SerializeField] private VRG_5sScore m_5sScore = null;
        

        /// <summary>
        /// Validate all the From: Components variables, if there is a null, it destroy itself
        /// </summary>
        private void Awake()
        {
            // Check that the object is well configured
            bool bDestroy = false;
            if (this.m_5sTimer == null)
            {
                bDestroy = true;
                this.Logs("VRG_5sTimer IS NULL", ENUM_Verbose.ERROR);
            }

            if (this.m_5sMap == null)
            {
                bDestroy = true;
                this.Logs("VRG_5sMap IS NULL", ENUM_Verbose.ERROR);
            }

            if (this.m_5sTimerAdd == null)
            {
                bDestroy = true;
                this.Logs("VRG_5sTimerAdd IS NULL", ENUM_Verbose.ERROR);
            }

            if (this.m_5sTimerMax == null)
            {
                bDestroy = true;
                this.Logs("VRG_5sTimerMax IS NULL", ENUM_Verbose.ERROR);
            }

            if (this.m_5sTimerLess == null)
            {
                bDestroy = true;
                this.Logs("m_5sTimerLess IS NULL", ENUM_Verbose.ERROR);
            }

            if (this.m_5sScore == null)
            {
                bDestroy = true;
                this.Logs("m_5sScore IS NULL", ENUM_Verbose.ERROR);
            }

            if (bDestroy)
            {
                Object.Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// To it's thing modifiy the difficulty settings according with the campaign->current data 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            int iValue = VRG_Session.GetInt("Campaign", "Current");

            VRG_5sDifficultyConfiguration vrgConfig = null;

            switch (iValue)
            {
                case 1:
                    if (this.m_Sleeper != null)
                    {
                        vrgConfig = this.m_Sleeper;
                    }
                    else
                    {
                        this.Logs("Sleepers difficulty is NULL", ENUM_Verbose.ERROR);
                    }
                    break;

                case 2:
                    if (this.m_Aware != null)
                    {
                        vrgConfig = this.m_Aware;
                    }
                    else
                    {
                        this.Logs("Aware difficulty is NULL", ENUM_Verbose.ERROR);
                    }
                    break;

                case 3:
                    if (this.m_Enlighten != null)
                    {
                        vrgConfig = this.m_Enlighten;
                    }
                    else
                    {
                        this.Logs("Enlighten difficulty is NULL", ENUM_Verbose.ERROR);
                    }
                    break;

                case 4:
                    if (this.m_Aware != null)
                    {
                        vrgConfig = this.m_Awaken;
                    }
                    else
                    {
                        this.Logs("Awaken difficulty is NULL", ENUM_Verbose.ERROR);
                    }
                    break;
            }

            // Copy the current configuration into the game items
            if (vrgConfig != null)
            {
                this.m_5sTimer.winTime = vrgConfig.winTime;
                this.m_5sTimer.winRound = vrgConfig.winRound;// == 1? 20 : 0;

                this.m_5sMap.starChance = vrgConfig.starChance;

                this.m_5sTimerAdd.amount = vrgConfig.timeBonus;
                this.m_5sTimerMax.amount = vrgConfig.timeMaxBonus;

                this.m_5sTimerLess.amount = vrgConfig.timeLessPerRound;

                this.m_5sMap.duration = vrgConfig.duration;
                this.m_5sMap.changeColor = vrgConfig.colorChange == 0? false : true;

                this.m_5sScore.starMultiplier = vrgConfig.starScore;
                this.m_5sScore.bonusMultiplier = vrgConfig.bonusScore;
            }

            // go to next frame
            yield return null;
        }

    }
}