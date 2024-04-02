using UnityEngine;

using VrGamesDev;
using VrGamesDev.BHEL;
using VrGamesDev.DDuA;


// <summary>
// This class controls the mechanics of the example DDuA/03 Spawner
// Custom the duration and number of random spawns before they dissapears
// </summary>

///#IGNORE
public class Example_WhenComplete : MonoBehaviour
{
    [Tooltip("asdasdasd")]
    [SerializeField]
    protected VRG_Addressable m_Addressable = null;



    private void OnEnable()
    {
        this.m_Addressable.WhenCreated += WhenCreated;
    }
    private void OnDisable()
    {
        this.m_Addressable.WhenCreated -= WhenCreated;
    }

    private void WhenCreated(GameObject valueLocal)
    {
        if (valueLocal != null)
        {
            Color myColor = VRG.GetRandomColor();

            valueLocal.name = valueLocal.name + " - " + VRG.GetHtmlFromColor(myColor);

            // set a random color
            valueLocal.GetComponent<Renderer>().material.SetColor("_Color", myColor);

            VRG_Bhel.Do
            (
                "<color=" + VRG.GetHtmlFromColor(myColor) + ">" + valueLocal.name + "</color> renamed and colorized with <color=" + VRG.GetHtmlFromColor(myColor) + ">" + VRG.GetHtmlFromColor(myColor) + "</color> color",
                "Example_WhenComplete->WhenCreated()",
                ENUM_Verbose.LOGS,
                VRG.GetSceneGameObject(valueLocal)
            );
        }
    }
}