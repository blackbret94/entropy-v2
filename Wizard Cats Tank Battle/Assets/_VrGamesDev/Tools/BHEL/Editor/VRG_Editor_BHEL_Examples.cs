using UnityEditor;

///#IGNORE
//  This namespace is the base to all the editor classes of VRG packages
namespace VrGamesDev.Editor
{
    public class VRG_Editor_BHEL_Examples : VRG_Editor
    {
        public new static string m_Prefabs = "Tools/BHEL/Prefabs/";

        /*
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/Clear Examples Data", false, 110001)]
        public static void Example_110001()
        {
            print("BHEL = Example_110001()");
        }
        */

/*
Add the script component <i><b>VRG_Bhel_Log.cs</b></i> to any gameObject, to add an entry to the BHEL logs, if you do not fill the "<i>value</i>" property, you will log the name of the gameObject, in this example you will have a "<b>Hello world</b>" in the Html.


Press the "Open Bhel" button to see your log in your default browser
        */

        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/00 Hello World", false, 111000)]
        public static void Example_111000() => LoadScene("BHEL/Examples/Scenes/00 Hello World");

/*
You can set the value property of the <i><b>VRG_Bhel_Log</b></i>, to add an entry in the log with that value.

In this example you can refresh your BHEL html, it will add an entry every second, ping-pong it between both objects.
*/
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/01 Ping Pong", false, 111001)]
        public static void Example_111001() => LoadScene("BHEL/Examples/Scenes/01 Ping Pong");

/*
You can decide to show the logs in the HTML, CSV, UI or the unity console.

Modify the property <i><b>showInConsole</b></i> to display the logs in the console too.

Press the button to change the showInConsole property ON / OFF         
*/
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/02 Console", false, 111002)]
        public static void Example_111002() => LoadScene("BHEL/Examples/Scenes/02 Console");


/*
You can add entries to the log using the <i><b>Do</b></i> Method from class VRG_Bhel just add the header:

<i><b>using VrGamesDev.BHEL;</b></i>

Then call the "<i>Do</i>" method with the message you want to log.

<i><b>VRG_Bhel.Do("Awake Example_BhelDo");</b></i>
*/
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/03 Do Method", false, 111003)]
        public static void Example_111003() => LoadScene("BHEL/Examples/Scenes/03 Do Method");

        /*
In the previous example, you have a <color=red><i>"N/A"</i></color> in the scene column, you can fill all data needed to have more detailed log:

VRG_Bhel.Do(<i>value, fromWhere, ENUM_Verbose, gameObject</i>);

<b>value</b>: The string message to send to the log file
<b>fromWhere</b>: Helps to understand who summon the log
<b>ENUM_Verbose</b>: Custom Verbose level (colored)
<b>gameObject</b>: The object that summons this
        */
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/04 Detailed", false, 111004)]
        public static void Example_111004() => LoadScene("BHEL/Examples/Scenes/04 Detailed");

        /*
You can also inheritance from the VRG_Base class,

<i><b>public class Example_Inheritance : VRG_Base</b></i>

Then call the Logs method with the message you want to log.

<i><b>this.Logs("Awake FromInheritance");</b></i>

Check the script <i>Example_Inheritance.cs</i> for a sample of this code 
*/
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/05 Inherintance", false, 111005)]
        public static void Example_111005() => LoadScene("BHEL/Examples/Scenes/05 Inherintance");

        /*
The level of verbose you set in your entries is what logs:

<color=black><i>NONE</i></color>: NONE is always sent to the editor log window
<color=red><i>ERROR</i></color>: Inform of failure
<color=yellow><i>WARNING</i></color>: Something was cheesy, if something is wacky, strange, or unexpected
<color=green><i>LOGS</i></color>: To track some basic information
<color=blue><i>DEBUG</i></color>: More information, ins and outs, flow of the game, useful for debug using asynchronous activities
<color=cyan><i>ALL</i></color>: The most verbose, it basically shows EVERYTHING!!!
        */
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/06 Verbose", false, 111006)]
        public static void Example_111006() => LoadScene("BHEL/Examples/Scenes/06 Verbose");

        /*
This example uses the Remote Config module (VRG_Remote prefab) and it allows you to change the data in run time.
Before running it, check the data in the VRH_Bhel Prefab, it will change with the VRG_Remote settings:
        */
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/07 VRG_Remote", false, 111007)]
        public static void Example_111007() => LoadScene("BHEL/Examples/Scenes/07 VRG_Remote");

        /*
What do you mean with "Everyone" ?

Exactly that, you get all the previous examples together, you get the hello world, ping pong, detailed, verbose and the VRG_Remote together.

Pay attention, the append mode is configured in the VRG_Remote prefab, and its set to create a new log every time you run it in the folder <i>BHEL_FromRemote</i>, remember to delete the logs when you are done testing.
        */
        [MenuItem("Tools/Vr Games Dev/Examples/BHEL/08 EVERRRRRYOOOOONE", false, 111008)]
        public static void Example_111008() => LoadScene("BHEL/Examples/Scenes/08 Bring Me Everyone");
    }
}