using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Cooldhands.UICursor.Example
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Image m_fillImage;
        public float m_waitTimeToHold = 0.2f;
        public UnityEvent OnHold;

        private float m_waitTime;
        private float m_progressWait = 1f;
        private bool m_holdAction;
        private bool m_holdFinish = false;

        //Detect current clicks on the GameObject (the one with the script attached)
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            m_waitTime = Time.time + m_waitTimeToHold;
            m_holdAction = true;
            m_holdFinish = false;
        }

        //Do this when the cursor exits the rect area of this selectable UI object.
        public void OnPointerExit(PointerEventData eventData)
        {
            m_holdAction = false;
        }

        //Detect if clicks are no longer registering
        public void OnPointerUp(PointerEventData pointerEventData)
        {
            m_holdAction = false;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if(m_holdAction)
            {
                if(Time.time > m_waitTime && !m_holdFinish)
                {
                    m_fillImage.fillAmount += 1.0f / m_progressWait * Time.deltaTime;
                    if(m_fillImage.fillAmount >= 1f)
                    {
                        if(OnHold != null)
                        {
                            m_holdFinish = true;
                            OnHold.Invoke();
                        }
                    }
                }
            }
            else{
                if(m_fillImage.fillAmount > 0f)
                {
                    m_fillImage.fillAmount = 0f;
                }
            }
        }
    }
}
