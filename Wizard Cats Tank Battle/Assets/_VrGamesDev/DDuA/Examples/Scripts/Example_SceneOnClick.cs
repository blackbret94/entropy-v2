using UnityEngine;
using UnityEngine.UI;

using VrGamesDev.DDuA;

///#IGNORE
public class Example_SceneOnClick : MonoBehaviour
{
    [SerializeField]
    protected VRG_Addressable m_Addressable = null;

    [SerializeField]
    protected Text m_Text;

    [SerializeField]
    protected Slider m_Slider;

    [SerializeField]
    protected GameObject m_Download;

    [SerializeField]
    protected GameObject m_Apply;




    private void Awake()
    {
        if (false
            || this.m_Addressable == null
            || this.m_Text == null
            || this.m_Slider == null
            || this.m_Download == null
            || this.m_Apply == null
            )
        {
            Object.Destroy(this.gameObject);
        }

        this.m_Addressable.data.WhenComplete += Data_WhenComplete;
    }

    private void Data_WhenComplete()
    {
        switch (this.m_Addressable.data.status)
        {
            case ENUM_AddressableStatus.SIZED:
                this.m_Addressable.data.evolution = ENUM_AddressableEvolution.DOWNLOAD;
                break;

            case ENUM_AddressableStatus.DOWNLOADED:                
                this.m_Apply.SetActive(true);
                this.m_Download.SetActive(false);
                break;
        }
    }

    private void Update()
    {
        this.m_Text.text = this.m_Addressable.data.CurrentDownload();
        this.m_Slider.value = this.m_Addressable.data.CurrentDownloadPercentage();

        if (this.m_Slider.value > 0 && this.m_Slider.value < 1)
        {
            print("this.m_Slider.value = " + this.m_Slider.value);
        }

    }

    private void OnDestroy()
    {
        this.m_Addressable.data.WhenComplete -= Data_WhenComplete;
    }

}