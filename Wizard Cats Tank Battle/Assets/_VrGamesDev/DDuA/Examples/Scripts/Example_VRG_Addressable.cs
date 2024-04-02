using UnityEngine;
using UnityEngine.UI;


using VrGamesDev.DDuA;

///#IGNORE
public class Example_VRG_Addressable : MonoBehaviour
{
    [Tooltip("asdasdasd")]
    [SerializeField]
    protected GameObject m_Prefab = null;

    [Header("From: Components")]
    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_bhelStatus = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Dropdown m_AddressInput = null;




    [Header("From: UI")]
    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_Verbose = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_Evolution = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_Status = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_Path = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_Size = null;

    [Header("---  DEBUG: DO NOT EDIT below  ---")]
    [Tooltip("asdasdasd")]
    //[SerializeField]
    protected VRG_Addressable m_Addressable = null;





    // Start is called before the first frame update
    void Start()
    {
        if (this.m_Prefab != null && this.m_AddressInput != null)
        {
            this.m_Addressable = Object.Instantiate(this.m_Prefab.gameObject).GetComponent<VRG_Addressable>();

            this.m_Addressable.name = this.m_Addressable.name.Replace("(clone)", "");

            this.m_Addressable.transform.position = new Vector3(2.5f, 0.0f, 0.0f);

            this.m_Addressable.SetAddress(this.m_AddressInput.options[this.m_AddressInput.value].text);
        }
        else
        {
            Debug.LogError("this.m_Prefab and this.m_Address should be non-null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.m_Verbose != null)
        {
            this.m_Verbose.text = this.m_Addressable.data.verbose.ToString();
        }

        if (this.m_Evolution != null)
        {
            this.m_Evolution.text = this.m_Addressable.data.evolution.ToString();
        }

        if (this.m_Status != null)
        {
            this.m_Status.text = this.m_Addressable.data.status.ToString();
        }

        if (this.m_Path != null)
        {
            this.m_Path.text = this.m_Addressable.data.path.ToString();
        }

        if (this.m_Size != null)
        {
            this.m_Size.text = this.m_Addressable.data.size.ToString();
        }

    }

    // Update is called once per frame
    public void Play()
    {
        this.m_Addressable.Play(this.m_AddressInput.options[this.m_AddressInput.value].text);
    }

    // Update is called once per frame
    public void Destroy()
    {
        this.m_Addressable.Destroy();
    }


    public void ToogleLocked()
    {
        this.m_Addressable.ToogleLocked();
    }

    public void ToogleParent()
    {
        this.m_Addressable.ToogleParent();
    }

    public void ToogleOverwrite()
    {
        this.m_Addressable.ToogleOverwrite();
    }



}