using System.Collections;

using UnityEngine;

using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Grow a graphical number used in the score
    /// </summary>
    public class VRG_5sScore : VRG_GrowingNumber
    {
        /// #IGNORE
        [Tooltip("The amount to multiply the stars collected")]
        [SerializeField] private float m_StarMultiplier = 5.0f;
        /// <summary>
        /// The amount to multiply the stars collected
        /// </summary>
        public float starMultiplier { get { return this.m_StarMultiplier; } set { this.m_StarMultiplier = value; } }

        /// #IGNORE
        [Tooltip("The amount to multiply the bonus time collected")]
        [SerializeField] private float m_BonusMultiplier = 10.0f;
        /// <summary>
        /// The amount to multiply the bonus time collected
        /// </summary>
        public float bonusMultiplier { get { return this.m_BonusMultiplier; } set { this.m_BonusMultiplier = value; } }

        [Header("FROM: Components")]
        /// <summary>
        /// The Graphical Number to modify
        /// </summary>
        [Tooltip("The Graphical Number to modify")]
        [SerializeField] private VRG_GraphicalNumber m_GraphicalNumber = null;

        /// <summary>
        /// The source data for the checks
        /// </summary>
        [Tooltip("The source data for the checks")]
        [SerializeField] private Text m_Pass = null;

        /// <summary>
        /// The source data for the stars
        /// </summary>
        [Tooltip("The source data for the stars")]
        [SerializeField] private Text m_Star = null;

        /// <summary>
        /// The source data for the bonus time
        /// </summary>
        [Tooltip("The source data for the bonus time")]
        [SerializeField] private Text m_Bonus = null;




        /// #IGNORE
        // Do not init the text data
        private void Awake() {}

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Calculate the score and display it, take into consideration the stars and checks
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Do()
        {
            // the speed to calculate
            float fSpeed;

            // the number to displau currentyl
            float fNumberCurrent = 0;

            // after calculating the floating point
            float fNumberNormalized = 0;

            // the last number displayed
            float fNumberPrevious;

            // calculate the final number to give the bonus
            int fNumberFinal = Mathf.RoundToInt(1
                * float.Parse(this.m_Pass.text)
                * (float.Parse(this.m_Star.text) <= 0 ? 1 : (float.Parse(this.m_Star.text) * this.m_StarMultiplier))
                * float.Parse(this.m_Bonus.text) * this.m_BonusMultiplier
                );

            // the speed by the duration
            fSpeed = fNumberFinal / this.m_Duration;

            // repeat until the final number is reached
            while (fNumberCurrent < fNumberFinal)
            {
                // save the previoous number to calculate the "OnAdd" event
                fNumberPrevious = fNumberNormalized;

                // get the number without the tiny increments
                fNumberNormalized = float.Parse(fNumberCurrent.ToString("F" + this.m_Decimals.ToString()));

                // set the value into the graphical number
                this.m_GraphicalNumber.SetNumber(int.Parse(fNumberNormalized.ToString()));
                fNumberCurrent += Time.deltaTime * fSpeed;

                // if it changed
                if (fNumberPrevious != fNumberNormalized)
                {
                    // cycle the OnAdd event
                    foreach (GameObject child in this.m_WhenAdd)
                    {
                        // activate it
                        child.SetActive(true);
                    }
                }

                // next frame
                yield return null;
            }

            // make sure the number is exact to the final number
            this.m_GraphicalNumber.SetNumber(fNumberFinal);

            // cycle the On Done game Objects
            foreach (GameObject child in this.m_WhenDone)
            {
                // activate it
                child.SetActive(true);
            }

            // save the record if higher than previous
            if (fNumberFinal > VRG_Session.GetInt("Campaign", "StarTotal"))
            {
                VRG_Session.SetInt("Campaign", "StarTotal", fNumberFinal);
            }

            // next frame
            yield return null;
        }

    }
}