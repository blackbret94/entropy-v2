using UnityEngine;

using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// #IGNORE
    /// <summary>
    /// Display the FPS in the screen
    /// </summary>
    public class VRG_FPS : VRG_Base
    {
        /// <summary>
        /// The text UI component
        /// </summary>
        [SerializeField] private Text m_Text;

        [Tooltip("The display format")]
        //[SerializeField]
        private string m_Display = "{0} ";// ({1})";

        [Tooltip("The FPS buffer index")]
        //[SerializeField]
        private int m_BufferIndex = 0;

        [Tooltip("The FPS buffer")]
        //[SerializeField]
        private float[] m_Buffer = new float[60];

        [Tooltip("The FPS buffer average")]
        //[SerializeField]
        private float m_FPSAverage = 0.0f;

        [Tooltip("The FPS buffer average")]
        //[SerializeField]
        private float m_FPSMax = 99.9f;





        /// <summary>
        /// Singleton pattern, Instance property should be the unique class in the scene
        /// </summary>
        public static VRG_FPS Instance; void Awake()
        {
            // Checking if I am the first instance
            if (Instance == null)
            {
                // I am the one
                Instance = this;

                // i follow my own rules
                this.transform.SetParent(null);

                // ... and I will live forever
                DontDestroyOnLoad(this);
            }
            else
            {
                // I am not worthy
                Destroy(this.gameObject);
            }
        }



        private void Start()
        {
            //find the text to display
            this.m_Text = this.FindMy(this.m_Text, false);

            // check if there is a frame rate set
            if (Application.targetFrameRate > 0)
            {
                this.m_FPSMax = Application.targetFrameRate;
            }
        }

        private void Update()
        {
            // save the current frame in the buffer
            this.m_Buffer[this.m_BufferIndex++] = 1f / Time.unscaledDeltaTime;

            // restart the buffer
            if (this.m_BufferIndex >= 60)
            {
                this.m_BufferIndex = 0;
            }

            // add the last 60 frames to get a smooth average
            int iCount = 0;
            this.m_FPSAverage = 0.0f;
            foreach (float child in this.m_Buffer)
            {
                if (child > 0)
                {
                    iCount++;
                    this.m_FPSAverage += child;
                }
            }

            // display with format and rounded to 1 digit
            this.m_Text.text = string.Format
            (
                this.m_Display,
                (Mathf.Clamp(this.m_FPSAverage / iCount, 0.0f, this.m_FPSMax)).ToString("F1")
            );

        }
    }
}