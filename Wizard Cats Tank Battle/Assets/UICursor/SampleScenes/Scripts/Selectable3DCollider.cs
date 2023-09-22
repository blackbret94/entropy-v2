using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace Cooldhands.UICursor.Example
{
    [RequireComponent(typeof(Renderer))]
    public class Selectable3DCollider : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler,  IPointerDownHandler, ISelectHandler, IDeselectHandler
    {
        private Renderer _rend;
        private Color _defaultColor;

        public Color SelectedColor = Color.red;
        public Color OverColor = Color.cyan;


        void Start() {
            _rend = GetComponent<Renderer>();
            _defaultColor = _rend.material.color;
        }

        //Do this when the gameobject (collider) is selected.
        public void OnSelect(BaseEventData eventData)
        {
            _rend.material.color = SelectedColor;
        }

         //Do this when the gameobject (collider) is deselected.
        public void OnDeselect(BaseEventData eventData)
        {
            _rend.material.color = _defaultColor;
        }

        //Do this when the cursor exits the rect area of this selectable UI object.
        public void OnPointerExit(PointerEventData eventData)
        {
            if (EventSystem.current.currentSelectedGameObject != this.gameObject)
            {
                _rend.material.color = _defaultColor;
            }
        }

         //Do this when the cursor enters the rect area of this selectable UI object.
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current.currentSelectedGameObject != this.gameObject)
            {
                _rend.material.color = OverColor;
            }
        }

        //Detect current clicks on the GameObject (the one with the script attached)
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject, pointerEventData);
        }
    }
}
