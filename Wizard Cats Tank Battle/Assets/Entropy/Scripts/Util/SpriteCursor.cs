using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.Util
{
    public class SpriteCursor : MonoBehaviour
    {
        public Canvas parentCanvas;
        public RawImage mouseCursor;

        public void Start()
        {
            Cursor.visible = false;
        }
        
        public void Update()
        {
            Vector2 movePos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition, parentCanvas.worldCamera,
                out movePos);

            Vector3 mousePos = parentCanvas.transform.TransformPoint(movePos);

            //Set fake mouse Cursor
            mouseCursor.transform.position = mousePos;

            //Move the Object/Panel
            transform.position = mousePos;
        }
    }
}