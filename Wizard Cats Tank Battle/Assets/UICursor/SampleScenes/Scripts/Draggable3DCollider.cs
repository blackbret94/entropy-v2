using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Draggable3DCollider : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Rigidbody _ridigBody = null;
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private Vector3 _cursorPosition;
    private Camera _camera;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        _ridigBody = GetComponent<Rigidbody>();
        _camera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(_camera != null)
        {
            _screenPoint = _camera.WorldToScreenPoint(transform.position);
            _offset = transform.position - _camera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(_camera != null)
        {
            Vector3 cursorPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
            _cursorPosition = _camera.ScreenToWorldPoint(cursorPoint) + _offset;

            if (_ridigBody == null)
            {
                transform.position = _cursorPosition;
            }
            else
            {
                _ridigBody.MovePosition(_cursorPosition);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
