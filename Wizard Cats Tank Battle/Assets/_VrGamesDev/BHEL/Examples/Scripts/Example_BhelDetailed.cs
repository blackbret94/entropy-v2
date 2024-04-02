using UnityEngine;

using UnityEngine.UI;

using VrGamesDev;
using VrGamesDev.BHEL;

/// #IGNORE
public class Example_BhelDetailed : MonoBehaviour
{
    [Tooltip("Level of verbose, NONE for silence")]
    [SerializeField] protected ENUM_Verbose m_Verbose = ENUM_Verbose.ALL;
    public ENUM_Verbose verbose { get { return this.m_Verbose; } set { this.m_Verbose = value; } }

    public Text m_FPS = null;
    public Text m_Time = null;
    public InputField m_Text = null;

    public Text m_Saved = null;

    private void Awake()
    {
        VRG_Bhel.Do
        (
            "Awake Example_BhelDetailed",
            "Example_BhelDetailed->Awake()",
            this.m_Verbose,
            VRG.GetSceneGameObject(this.gameObject)
        );
    }

    private void Start()
    {
        VRG_Bhel.Do
        (
            "Start Example_BhelDetailed",
            "Example_BhelDetailed->Start()",
            this.m_Verbose,
            VRG.GetSceneGameObject(this.gameObject)
        );
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
            this.m_Time.text = Time.fixedUnscaledTime.ToString("F6");
        }
    }

    public void BHEL()
    {
        this.BHEL(Random.Range(0, 5));
    }

    public void BHEL(int valueLocal)
    {
        VRG_Bhel.Do
        (
            this.m_Text.text.ToString(),
            "Example_BhelDetailed->BHEL()",
            (ENUM_Verbose) valueLocal,
            VRG.GetSceneGameObject(this.gameObject)
        );

        this.m_Text.text = string.Empty;

        if (this.m_Saved != null)
        {
            this.m_Saved.gameObject.SetActive(true);
        }
    }

}