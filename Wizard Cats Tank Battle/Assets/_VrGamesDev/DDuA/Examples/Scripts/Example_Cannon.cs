using UnityEngine;

using VrGamesDev;
using VrGamesDev.BHEL;
using VrGamesDev.DDuA;


// <summary>
// This class controls the mechanics of the example DDuA/03 Spawner
// Custom the duration and number of random spawns before they dissapears
// </summary>

///#IGNORE
public class Example_Cannon : MonoBehaviour
{
    [Header("From: Components")]
    [Space(5)]
    [Tooltip("asdasdasd")]
    [SerializeField]
    protected VRG_Addressable m_Addressable = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected GameObject m_Rifle = null;

    [Tooltip("asdasdasd")]
    [SerializeField]
    protected GameObject m_Scope = null;





    [Header("From: Configuration")]
    [Space(20)]
    [Tooltip("asdasdasd")]
    [SerializeField]
    protected float m_Speed = 50.0f;


    private void Update()
    {
        this.m_Rifle.transform.Rotate(Input.GetAxis("Horizontal") * this.m_Speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * this.m_Speed * Time.deltaTime);

        Vector3 myEulers = this.m_Rifle.transform.eulerAngles;
        if (myEulers.z < 270)
        {
            myEulers.z = 270;
        }
        else if (myEulers.z > 340)
        {
            myEulers.z = 340;
        }

        this.m_Rifle.transform.eulerAngles = myEulers;

        if (Input.GetMouseButtonUp(0))
        {
            GameObject myObject = this.m_Addressable.Get();

            if (myObject != null)
            {
                myObject.transform.position = this.m_Scope.transform.position;
                myObject.transform.rotation = this.m_Scope.transform.rotation;

                myObject.GetComponent<Example_Bullet>().Play();

            }
        }
    }
}
























// Get the horizontal and vertical axis.
// By default they are mapped to the arrow keys.
// The value is in the range -1 to 1
//float fMoveInX = Input.GetAxis("Horizontal") * this.m_Speed * Time.deltaTime;


/*
if (this.m_Rifle.transform.localRotation.z < this.m_Horizontal.x || this.m_Rifle.transform.localRotation.z > this.m_Horizontal.y)
{
    fMoveInX = 0;
}
if (this.m_Rifle.transform.localRotation.z < this.m_Vertical.x || this.m_Rifle.transform.localRotation.z > this.m_Vertical.y)
{
    fMoveInZ = 0;
}
*/

/*
float fMoveInZ = 0;
if (this.m_Rifle.transform.rotation.z < this.m_Vertical.x && this.m_Rifle.transform.rotation.z > this.m_Vertical.y)
{
    fMoveInZ = Input.GetAxis("Vertical") * this.m_Speed * Time.deltaTime;
}

print((this.m_Rifle.transform.rotation.z > this.m_Vertical.x && this.m_Rifle.transform.rotation.z < this.m_Vertical.y) + " | " +this.m_Vertical.x + " < " + this.m_Rifle.transform.rotation.z + " > " + this.m_Vertical.y);

fMoveInX = 0;
this.m_Rifle.transform.Rotate(fMoveInX, 0, fMoveInZ);

*/

/*
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
    print(valueLocal);

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
*/