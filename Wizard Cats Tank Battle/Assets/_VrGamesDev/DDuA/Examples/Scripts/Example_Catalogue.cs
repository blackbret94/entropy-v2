using UnityEngine;
using UnityEngine.UI;


using VrGamesDev;
using VrGamesDev.BHEL;
using VrGamesDev.DDuA;

///#IGNORE
public class Example_Catalogue : MonoBehaviour
{
    [Tooltip("asdasdasd")]
    [SerializeField]
    protected VRG_DDuA m_DDuA = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_Title = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_List = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected GameObject m_Controls = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected Text m_Details = null;

    [SerializeField]
    private bool m_Next = false;
    public bool next
    {
        get
        {
            return this.m_Next;
        }
        set
        {
            this.m_Next = value;
        }
    }

    [SerializeField]
    public bool m_AllorStop = false;
    public bool allorStop
    {
        get
        {
            return this.m_AllorStop;
        }
        set
        {
            this.m_AllorStop = value;
        }
    }

    [SerializeField]
    public bool m_Disable = false;
    public bool disable
    {
        get
        {
            return this.m_Disable;
        }
        set
        {
            this.m_Disable = value;
        }
    }


    private void OnEnable()
    {
        if (this.m_DDuA == null)
        {
            this.m_DDuA = Object.FindObjectOfType<VRG_DDuA>();
        }

        if (false
            || this.m_DDuA == null
            || this.m_Title == null
            || this.m_List == null
            || this.m_Controls == null
            || this.m_Details == null
            
            )
        {
            VRG_Bhel.Do("There are some null components, please redownload the DDuA Package", "Example_Catalogue->Do()", VrGamesDev.ENUM_Verbose.ERROR, VRG.GetSceneGameObject(this.gameObject));
            Object.Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        this.m_Title.text = string.Format("<b>Queue:</b> {0} / <b>Size:</b> {1}", this.m_DDuA.catalogue.Count, VRG.LongToBytes(this.m_DDuA.catalogueSize));

        if (this.m_DDuA.catalogue.Count > 0)
        {
            string sList = "";
            foreach (VRG_AddressableSerializable child in this.m_DDuA.catalogue)
            {
                sList += " - " + child.addressFromPath + " = <color=white>" + VRG.LongToBytes(child.size) + "</color>\n";
            }
            this.m_List.text = sList;

            this.m_Details.text = ""
                + "Address: <color=white>" + this.m_DDuA.catalogue[0].addressFromPath + "</color>" + "\n"
                + "Status: <color=white>" + this.m_DDuA.catalogue[0].status + "</color>" + "\n"
                + "Size: <color=white>" + this.m_DDuA.catalogue[0].size + "</color>" + "\n"
                + "\n"
                + "Hash: \n<color=white>" + this.m_DDuA.catalogue[0].address + "</color>" + "\n"
                + "\n"
                + "Path: \n<color=white>" + this.m_DDuA.catalogue[0].path + "</color>" + "\n"
                ;



            if (this.m_Next)
            {
                this.m_DDuA.isCatalogueDownload = this.m_Next;
                //print(Time.frameCount + ") DOWNLOAD");

                this.m_Next = false;

                if (!this.m_AllorStop)
                {
                    this.m_Disable = true;
                }
            }
            else
            {
                if (this.m_Disable)
                {
                    this.m_Disable = false;
                    this.m_DDuA.isCatalogueDownload = this.m_Next;
                    //print(Time.frameCount + ") STOP");
                }
            }
        }
        else
        {
            this.m_Controls.SetActive(false);

            this.m_List.text = "List is empty";
            this.m_List.fontSize = 40;

            this.m_Details.text = "The game download is up to date and all assets are cached";
            this.m_Details.fontSize = 40;
        }

    }

}