using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// The button in the page search for any image that matches the id of the button
    /// and replace the text with the image, useful to create custom buttons with images
    /// Check the <a href="#VrGamesDev.FiveSeconds">FiveSeconds</a> for an example how
    /// to use it.
    /// </summary>
    public class VRG_MissionPageButtonTitles : VRG_Base
    {

        /// <summary>
        /// The star game object
        /// </summary>
        [Tooltip("The star game object")]
        [SerializeField] private List<Transform> m_Childs = new List<Transform>();


        /// <summary>
        /// Singleton pattern, Instance is the unique variable in the scene from this class
        /// </summary>
        public static VRG_MissionPageButtonTitles Instance;

        /// #IGNORE
        private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // get all the childs to get the get
                foreach(Transform child in this.transform)
                {
                    this.m_Childs.Add(child);
                }

                // i follow my own rules
                this.transform.SetParent(null);

                // ... and I will live forever
                DontDestroyOnLoad(this);
            }
            else
            {
                // I am not the one the prophecy said ... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }

        /// #IGNORE
        protected override IEnumerator Do()
        {
            yield return null;
        }

        /// <summary>
        /// Get the first child image of this object
        /// </summary>
        /// <param name="valueLocal">The name of the Image (child gameObject)</param>
        /// <returns>An UI Image</returns>
        public Image Get(string valueLocal)
        {
            Image tReturn = null;

            foreach(Transform child in this.m_Childs)
            {
                if (child.name == valueLocal)
                {
                    GameObject instance = GameObject.Instantiate(child.gameObject) as GameObject;

                    tReturn = instance.GetComponent<Image>();
                }
            }

            return tReturn;
        }


    }
}