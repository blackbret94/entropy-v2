using System.Collections;
using UnityEngine;

using UnityEngine.UI;

namespace VrGamesDev
{
    /// #IGNORE
    public class Example_Inheritance : VRG_Base
    {
        public Text m_FPS = null;
        public Text m_Time = null;
        public InputField m_Text = null;

        public Text m_Saved = null;

        private void Awake()
        {
            this.Logs("Awake FromInheritance", "Example_Inheritance->Awake()");
        }

        private void Start()
        {
            this.Logs("Start FromInheritance", "Example_Inheritance->Start()");
        }

        // Update is called once per frame
        void Update()
        {
            if (this.m_FPS != null)
            {
                this.m_FPS.text = Time.frameCount.ToString();
            }

            if (this.m_Time != null)
            {
                this.m_Time.text = Time.time.ToString("F6");
            }
        }

        protected override IEnumerator Do()
        {
            string sLog = this.m_Text.text.ToString();

            if (sLog.Trim() == "")
            {
                sLog = "Inside the Do function, when calling Play()";
            }

            this.Logs(sLog, "Example_FromInheritance->Do()");

            this.m_Text.text = "";

            this.m_Saved.gameObject.SetActive(true);

            yield return null;
        }

    }

}