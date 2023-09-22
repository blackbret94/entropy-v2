using UnityEngine;
using System.Collections;
using UnityEditor;
using Cooldhands;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUICursor {
	
	[MenuItem("GameObject/UICursor/Event System", false, 0)]
	static void CreateMenuUICursor() {
		CreateEventSystem();
	}

	static void CreateEventSystem() {

		EventSystem[] evPrevs = (EventSystem[])Resources.FindObjectsOfTypeAll(typeof(EventSystem));
		for (int i = 0; i < evPrevs.Length; i++) {
			evPrevs [i].gameObject.SetActive (false);
		}
			
		GameObject objCanvas = new GameObject("CursorCanvas");

		Canvas cv = objCanvas.AddComponent<Canvas>();
		cv.sortingOrder = 32767;
		cv.renderMode = RenderMode.ScreenSpaceOverlay;

		CanvasScaler cvs = objCanvas.AddComponent<CanvasScaler>();
		cvs.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
		cvs.screenMatchMode =  UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;

		GameObject objImg = new GameObject("Cursor");
		Image img = objImg.AddComponent<Image>();
		img.raycastTarget = false;
		img.preserveAspect = true;
		objImg.transform.SetParent (objCanvas.transform);
	
		RectTransform rect = img.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (50, 50);
		rect.position = new Vector2 (Screen.width/2, Screen.height/2);

		GameObject obj = new GameObject("EventSystem");
		EventSystem ev = obj.AddComponent<EventSystem>();
		ev.sendNavigationEvents = false;

		obj.AddComponent<UICursorStandaloneInputModule>();

		Selection.activeObject = obj;
		EditorGUIUtility.PingObject(obj);
	}
}