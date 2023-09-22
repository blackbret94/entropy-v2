using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FocusOnEnable : MonoBehaviour
{
    Button m_Button;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(Focus());
    }

    IEnumerator Focus()
    {
        yield return new WaitForSecondsRealtime(1f);
        m_Button.Select();
    }

    // Update is called once per frame
    void Awake()
    {
        m_Button= GetComponent<Button>();
    }
}
