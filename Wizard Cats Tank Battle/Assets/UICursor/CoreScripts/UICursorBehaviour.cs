using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cooldhands
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UICursorBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Cursor speed for movement
        /// </summary>
        [Tooltip("Cursor speed for movement")] 
        public float m_Speed = 450f;
        private RectTransform m_RectTransform;
        public GameObject ClickableElement { get; set; }

        public RectTransform RectTransform
        {
            get{
                return m_RectTransform;
            }
        }
        public bool IsOverClickeableElement
        {
            get{
                return ClickableElement != null ? true : false;
            }
        }

        protected virtual void Awake()
        {
            m_RectTransform= this.GetComponent<RectTransform>();
        }
        public abstract void OnClickableElementChanged();
        public abstract void OnClick();
    }
}