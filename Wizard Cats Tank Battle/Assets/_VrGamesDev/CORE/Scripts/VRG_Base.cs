using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using VrGamesDev.BHEL;


#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
using Sirenix.OdinInspector;
#endif

namespace VrGamesDev
{
    /// <summary>
    /// Mother class of all VrGamesDev framework, it contains all the common methods
    /// to all classes, it controls the flow of OnEnable and Play() and the different
    /// modules for internal comunication.
    /// 
    /// It is also useful when you debug and verbose control
    /// </summary>
    public class VRG_Base : MonoBehaviour
    {
        /*



        [Header("From: Components")]
        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Space(20)]



        public VRG_Base()
        {
            this.m_PlayOnEnable = true;
            this.m_NextFrame = true;
            this.m_SelfTurnOff = false;
        }




        ///#IGNORE
        protected override IEnumerator Do() {    yield return null;  }





        ///#IGNORE
        protected override IEnumerator Do()
        {
            yield return null;
        }



        */




        // [Header("From: VRG_Base")]

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        // Box with a centered title.
        /// #IGNORE
        [Space(5)]
        [ShowIfGroup("FromVrg_Base")]
        [BoxGroup("FromVrg_Base/From: VRG_Base", centerLabel: true)]
#endif
        /// #IGNORE
        [Tooltip("Level of verbose, NONE for silence")]
        [SerializeField]
        protected ENUM_Verbose m_Verbose = ENUM_Verbose.WARNING;
        /// <summary>
        /// <p>Level of verbose, there are 6 level of verbose from silence up to full debug</p>
        /// <p>ENUM_Verbose.NONE = Silence, NONE is sent to the editor log window</p>
        /// <p>ENUM_Verbose.<font color=red>ERROR</font> =  When you need to show an error to the user and record a posible failure</p>
        /// <p>ENUM_Verbose.<font color=yellow>WARNING</font> = Something was cheesy, if something is wacky, strange, or unexpected</p>
        /// <p>ENUM_Verbose.<font color=green>LOGS</font> =To track some basic information, or to send logs to BHEL</p>
        /// <p>ENUM_Verbose.<font color=blue>DEBUG</font> = More information, ins and outs, flow of the game, useful for debug using asynchronous activities</p>
        /// <p>ENUM_Verbose.<font color=cyan>ALL</font> = The most verbose, it basically shows EVERYTHING!!!</p>
        /// </summary>
        public ENUM_Verbose verbose { get { return this.m_Verbose; } set { this.m_Verbose = value; } }


#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowIfGroup("FromVrg_Base")]
        [BoxGroup("FromVrg_Base/From: VRG_Base")]
#endif
        /// <summary>
        /// The default behiavor is to play OnEnable, when false, you need to call method .Play() to do it's thing
        /// </summary>
        [Tooltip("The default behiavor is to play OnEnable, when false, you need to call method .Play() to do it's thing")]
        [SerializeField] protected bool m_PlayOnEnable = true;


#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowIfGroup("FromVrg_Base")]
        [BoxGroup("FromVrg_Base/From: VRG_Base")]
#endif
        /// <summary>
        /// If it will wait until the next frame, useful if used on coroutines, or waiting some elements or components are declared
        /// </summary>
        [Tooltip("If it will wait until the next frame, useful if used on coroutines, or waiting some elements or components are declared")]
        [SerializeField] protected bool m_NextFrame = false;


#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowIfGroup("FromVrg_Base")]
        [BoxGroup("FromVrg_Base/From: VRG_Base")]
#endif
        /// <summary>
        /// It will turn off itself after the Play() -> Do() methods are done
        /// </summary>
        [Tooltip("It will turn off itself after it's done")]
        [SerializeField] protected bool m_SelfTurnOff = false;





#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Component")] public bool Component;
        [ToggleGroup("Configuration")] public bool Configuration;
        [ToggleGroup("Data")] public bool Data;


        [ToggleGroup("Configuration")]
        public bool FromVrg_Base = false;
#endif











        /// <summary>
        /// If this.m_PlayOnEnable is true = Start the <strong><em>"do your thing"</em></strong> (The <em>"play()"</em> method)
        /// </summary>
        protected void OnEnable()
        {
            if (this.m_PlayOnEnable)
            {
                this.Play();
            }
        }



        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Override this method in your own classes
        /// </summary>
        public virtual void Play()
        {
            StartCoroutine(this.Play_IEnumerator());
        }



        /// <summary>
        /// Enumerator proxy, wait a frame, run the Do() coruotine and turn it off.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Play_IEnumerator()
        {
            // if it will wait until this current frame is over
            if (this.m_NextFrame)
            {
                yield return null;
            }

            yield return StartCoroutine(Do());

            // turn myself off if true
            this.transform.gameObject.SetActive(!this.m_SelfTurnOff);

            // return, it is like a void
            yield return null;
        }










        ///#IGNORE
        protected virtual IEnumerator Do()
        {
            throw new System.NotImplementedException("Inside of Base.Do() -> override this function in your class");
        }








        /// <summary>
        /// This method finds the closest typeOf(Object) valueLocal element in itself, child and parents
        /// and return the same kind of object 
        /// </summary>
        /// <param name="valueLocal">the data to search for</param>
        /// <param name="destroyLocal">[optional] TRUE by default, it destroy the object if not found</param>
        protected void FindMy(object valueLocal, bool destroyLocal) { }



        /// <summary>
        /// VRG_BHEL wrapper, this method, send any message to the VRG_BHEL singleton
        /// </summary>
        /// <param name="valueLocal">The message to log</param>
        /// <param name="sourceLocal">From where the messag was sent, Use the format <strong><em>Class->Method()</em></strong></param>
        /// <param name="ENUM_VerboseLocal">Custom Verbose level, to be logged it should be equal or higher than the one defined in the VRG_Remote</param>

        protected virtual void Logs(string valueLocal, string sourceLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            if (sourceLocal.Trim() == string.Empty)
            {
                sourceLocal = this.GetType().ToString();
            }

            VRG_Bhel.Do(valueLocal, sourceLocal, ENUM_VerboseLocal, VRG.GetSceneGameObject(this.gameObject));
        }


        ///#EXIT
        protected virtual void Logs()
            => this.Logs(this.name, "", this.m_Verbose);
        protected virtual void Logs(string valueLocal)
            => this.Logs(valueLocal, "", this.m_Verbose);
        protected virtual void Logs(string valueLocal, string sourceLocal)
            => this.Logs(valueLocal, sourceLocal, this.m_Verbose);
        protected virtual void Logs(string valueLocal, ENUM_Verbose ENUM_VerboseLocal)
            => this.Logs(valueLocal, "", ENUM_VerboseLocal);














































        protected GameObject FindMy(GameObject valueLocal) => this.FindMy(valueLocal, "A GameObject");
        protected GameObject FindMy(GameObject valueLocal, string nameLocal)
        {
            if (valueLocal == null)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + nameLocal, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Canvas FindMy(Canvas valueLocal) => this.FindMy(valueLocal, true);
        protected Canvas FindMy(Canvas valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Canvas";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Canvas>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Canvas>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Button FindMy(Button valueLocal) => this.FindMy(valueLocal, true);
        protected Button FindMy(Button valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Button";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Button>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Button>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Slider FindMy(Slider valueLocal) => this.FindMy(valueLocal, true);
        protected Slider FindMy(Slider valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Slider";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Slider>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Slider>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Rigidbody FindMy(Rigidbody valueLocal) => this.FindMy(valueLocal, true);
        protected Rigidbody FindMy(Rigidbody valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Rigidbody";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Rigidbody>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Rigidbody>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Renderer FindMy(Renderer valueLocal) => this.FindMy(valueLocal, true);
        protected Renderer FindMy(Renderer valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Renderer";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Renderer>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Renderer>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Image FindMy(Image valueLocal) => this.FindMy(valueLocal, true);
        protected Image FindMy(Image valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Image";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Image>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Image>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected RectTransform FindMy(RectTransform valueLocal) => this.FindMy(valueLocal, true);
        protected RectTransform FindMy(RectTransform valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "RectTransform";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<RectTransform>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<RectTransform>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected GridLayoutGroup FindMy(GridLayoutGroup valueLocal) => this.FindMy(valueLocal, true);
        protected GridLayoutGroup FindMy(GridLayoutGroup valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "GridLayoutGroup";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<GridLayoutGroup>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<GridLayoutGroup>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected AudioSource FindMy(AudioSource valueLocal) => this.FindMy(valueLocal, true);
        protected AudioSource FindMy(AudioSource valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "AudioSource";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<AudioSource>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<AudioSource>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Text FindMy(Text valueLocal) => this.FindMy(valueLocal, true);
        protected Text FindMy(Text valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Text";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Text>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Text>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

        protected Camera FindMy(Camera valueLocal) => this.FindMy(valueLocal, true);
        protected Camera FindMy(Camera valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "Camera";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<Camera>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<Camera>();
            }

            if (valueLocal == null)
            {
                valueLocal = Camera.main;
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;

        }

        protected VRG_Fader FindMy(VRG_Fader valueLocal) => this.FindMy(valueLocal, true);
        protected VRG_Fader FindMy(VRG_Fader valueLocal, bool destroyLocal)
        {
            string sObjectFinder = "";

            if (valueLocal == null)
            {
                sObjectFinder = "VRG_Fader";

                if (destroyLocal)
                {
                    this.Logs(sObjectFinder, ENUM_Verbose.WARNING);
                }

                valueLocal = GetComponentInChildren<VRG_Fader>();
            }

            if (valueLocal == null)
            {
                valueLocal = GetComponentInParent<VRG_Fader>();
            }

            if (valueLocal == null && destroyLocal)
            {
                this.Logs("<color=red><b>DESTROYED: </b></color>" + sObjectFinder, ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

            return valueLocal;
        }

    }
}
