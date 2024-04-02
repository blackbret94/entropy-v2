using System.Collections;

using UnityEngine;

namespace VrGamesDev.DDuA
{
    /// <summary>
    /// Exit the aplication
    /// </summary>
    public class VRG_Cache : VRG_Base
    {
        [SerializeField]
        private GameObject m_Exit = null;



        public VRG_Cache()
        {
            m_PlayOnEnable = false;
            m_NextFrame = false;
            m_SelfTurnOff = false;
        }

        /// <summary>
        /// Singleton pattern, Instance is the variable that save the data from every class.
        /// </summary>
        public static VRG_Cache Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    // I am not the one the prophecy said ... I will walk to the eternal darkness
                    Destroy(this.gameObject);
                }
            }
        }

        public override void Play()
        {
            Clear();
        }

        public static void Clear()
        {
            Cache currentCache = Caching.currentCacheForWriting;
            float sSpace = (currentCache.spaceOccupied / 1000000.0f);

            if (Caching.ClearCache())
            {
                string sLogs = string.Empty;
                if (sSpace > 0)
                {
                    sLogs += "Cache cleared: ";
                    sLogs += sSpace.ToString("#,#");
                    sLogs += " Mb";
                }
                else
                {
                    sLogs = "The Cache was empty";
                }
                print(sLogs);
            }
            else
            {
                print("<color=red>Couldn't clear Cache </color>");
            }

            if (Instance != null)
            {
                if (Instance.m_Exit != null)
                {
                    Instance.m_Exit.SetActive(true);
                }
            }
        }



    }
}