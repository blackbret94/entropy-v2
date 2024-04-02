using UnityEngine;

using UnityEngine.UI;

using VrGamesDev.BHEL;

/// #IGNORE
public class Example_BhelDo : MonoBehaviour
{
    public Text m_FPS = null;
    public Text m_Time = null;
    public InputField m_Text = null;

    public Text m_Saved = null;

    private void Awake()
    {
        VRG_Bhel.Do("Awake Example_BhelDo");
    }

    private void Start()
    {
        VRG_Bhel.Do("Start Example_BhelDo");
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
            this.m_Time.text = Time. time.ToString("F6");
        }
    }

    public void BHEL()
    {
        VRG_Bhel.Do(this.m_Text.text.ToString());

        this.m_Text.text = string.Empty;

        this.m_Saved.gameObject.SetActive(true);
    }

}