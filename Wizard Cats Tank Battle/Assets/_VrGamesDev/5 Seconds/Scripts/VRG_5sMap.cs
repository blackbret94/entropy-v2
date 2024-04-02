using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev.FiveSeconds
{
    /// <summary>
    /// Control the map of the X-Check-Stars
    /// </summary>
    public class VRG_5sMap : VRG_Base
    {
        /// #IGNORE
        [Tooltip("The chance from 0-5 to spawn 1 star, each number is a 20% chance")]
        [SerializeField] private bool m_ChangeColor = false;
        /// <summary>
        /// The chance from 0-5 to spawn 1 star, each number is a 20% chance
        /// </summary>
        public bool changeColor { get { return this.m_ChangeColor; } set { this.m_ChangeColor = value; } }

        /// #IGNORE
        [Tooltip("The chance from 0-5 to spawn 1 star, each number is a 20% chance")]
        [Range(0.0f, 5.0f)]
        [SerializeField] private float m_Duration = 0.0f;
        /// <summary>
        /// The chance from 0-5 to spawn 1 star, each number is a 20% chance
        /// </summary>
        public float duration { get { return this.m_Duration; } set { this.m_Duration = value; } }

        /// #IGNORE
        [Tooltip("The chance from 0-5 to spawn 1 star, each number is a 20% chance")]
        [Range(0, 5)]
        [SerializeField] private int m_StarChance = 1;
        /// <summary>
        /// The chance from 0-5 to spawn 1 star, each number is a 20% chance
        /// </summary>
        public int starChance { get { return this.m_StarChance; } set { this.m_StarChance = value; } }

        /// <summary>
        /// The fail buttons game Objects
        /// </summary>
        [Tooltip("The fail buttons game Objects")]
        [SerializeField] private GameObject[] m_FAIL = null;

        /// <summary>
        /// The pass buttons game Objects
        /// </summary>
        [Tooltip("The pass buttons game Objects")]
        [SerializeField] private GameObject[] m_PASS = null;

        /// <summary>
        /// The star buttons game Objects
        /// </summary>
        [Tooltip("The star buttons game Objects")]
        [SerializeField] private GameObject[] m_STAR = null;

        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] private GameObject[] m_OnDone = null;

        /// <summary>
        /// When the movement finish Activate the event OnFinish
        /// </summary>
        [Tooltip("When the movement finish Activate the event OnFinish")]
        [SerializeField] private Color32[] m_Colors = null;


        /// #IGNORE
        protected override IEnumerator Do()
        {
            //Hide it on start
            this.Hide();

            // go to next frame
            yield return null;
        }

        /// <summary>
        /// Hide and unable all the buttons
        /// </summary>
        public void Hide()
        {
            // cycle everybodyyyyy
            foreach (Transform child in this.transform)
            {
                // activate it
                child.gameObject.SetActive(true);

                // if an image exists
                if (child.gameObject.GetComponentsInChildren<Image>(true)[1] != null)
                {
                    // disable the image
                    child.gameObject.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(false);
                }

                // and make it not pushable
                child.gameObject.GetComponent<Button>().interactable = false;
            }

            // except the stars
            foreach (GameObject child in this.m_STAR)
            {
                // deactivate it
                child.SetActive(false);
            }
        }



        /// <summary>
        /// Show and enable all the buttons
        /// </summary>
        public void Show()
        {
            StopAllCoroutines();

            StartCoroutine(this.Show_IEnumerator());
        }
        /// #IGNORE
        private IEnumerator Show_IEnumerator()
        {
            if (this.changeColor && this.m_Colors.Length > 0)
            {
                // cycle everybodyyyyy
                foreach (Transform child in this.transform)
                {
                    // enabled the image 
                    child.GetComponentsInChildren<Image>(true)[2].color = this.m_Colors[Random.Range(0, this.m_Colors.Length)];
                }
            }

            // cycle everybodyyyyy
            foreach (GameObject child in this.m_FAIL)
            {
                // activate it
                child.SetActive(true);

                // enabled the image 
                //child.GetComponentsInChildren<Image>(true)[1].enabled = true;
                child.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);

                // and make it pushable
                child.GetComponent<Button>().interactable = true;
            }

            // cycle the m_PASS game Objects
            foreach (GameObject child in this.m_PASS)
            {
                // activate it
                child.SetActive(true);

                // roll a random place
                child.transform.SetSiblingIndex(Random.Range(0, 9));

                // enable the image of the check
                child.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);

                // and make it pushable
                child.GetComponent<Button>().interactable = true;
            }

            // the star is disabled by default
            bool bStar = false;

            // if there is a chance 
            if (this.m_StarChance > 0)
            {
                // roll it and compare it to the chance 
                if (Random.Range(0, 5) <= (this.m_StarChance - 1))
                {
                    // activate the star
                    bStar = true;

                    // and invalidate the first X
                    this.m_FAIL[0].SetActive(false);
                }
            }

            // cycle the stars
            foreach (GameObject child in this.m_STAR)
            {
                // activate it 
                child.SetActive(bStar);

                // roll a random place
                child.transform.SetSiblingIndex(Random.Range(0, 9));

                // enable the image of the star
                //child.GetComponentsInChildren<Image>(true)[1].enabled = true;
                child.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);

                // and make it pushable
                child.GetComponent<Button>().interactable = true;
            }

            // inform the OnDone objects we are done.
            foreach (GameObject child in this.m_OnDone)
            {
                // activate it
                child.SetActive(true);
            }

            // turn the duration off
            if (this.duration > 0)
            {
                // ... wait
                yield return new WaitForSeconds(this.m_Duration);

                // cycle everybodyyyyy
                foreach (Transform child in this.transform)
                {
                    // enabled the image 
                    child.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(false);
                }
            }

            // go to next frame
            yield return null;
        }


    }
}