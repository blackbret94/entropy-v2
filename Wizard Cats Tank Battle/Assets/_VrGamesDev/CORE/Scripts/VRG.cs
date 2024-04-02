using UnityEngine;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    ///#IGNORE
    public class SceneNameAttribute : PropertyAttribute { }

    ///#IGNORE
    public struct VRG_Remote_UserAttributes { }

    ///#IGNORE
    public struct VRG_Remote_AppAttributes { }

    // <summary>
    // Struct that save the local Booleans settings for the VRG_Remote, just for testing,
    // use the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@latest">Unity Remote Config</a> module
    // </summary>
    ///#IGNORE
    [System.Serializable]
    public class VRG_RemoteBool
    {
        public string name;
        public bool value;

        public VRG_RemoteBool()
        {
        }

        public VRG_RemoteBool(string nameLocal, bool valueLocal)
        {
            this.name = nameLocal;
            this.value = valueLocal;
        }
    }

    // <summary>
    // Struct that save the local Integers settings for the VRG_Remote, just for testing,
    // Use the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@latest">Unity Remote Config</a> module
    // </summary>
    ///#IGNORE
    [System.Serializable]
    public class VRG_RemoteInt
    {
        public string name;
        public int value;

        public VRG_RemoteInt()
        {
        }

        public VRG_RemoteInt(string nameLocal, int valueLocal)
        {
            this.name = nameLocal;
            this.value = valueLocal;
        }
    }

    // <summary>
    // Struct that save the local Floats settings for the VRG_Remote, just for testing,
    // Use the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@latest">Unity Remote Config</a> module
    // </summary>
    ///#IGNORE
    [System.Serializable]
    public class VRG_RemoteFloat
    {
        public string name;
        public float value;

        public VRG_RemoteFloat()
        {
        }

        public VRG_RemoteFloat(string nameLocal, float valueLocal)
        {
            this.name = nameLocal;
            this.value = valueLocal;
        }
    }

    // <summary>
    // Struct that save the local Strings settings for the VRG_Remote, just for testing,
    // Use the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@latest">Unity Remote Config</a> module
    // </summary>
    ///#IGNORE
    [System.Serializable]
    public class VRG_RemoteString
    {
        public string name;
        public string value;

        public VRG_RemoteString()
        {
        }

        public VRG_RemoteString(string nameLocal, string valueLocal)
        {
            this.name = nameLocal;
            this.value = valueLocal;
        }
    }

    /// <summary>
    /// Audio mixer channels, they should be the same as the main mixer audio channels
    /// you need to configure this volume in the <a href="https://docs.unity3d.com/Manual/AudioMixer.html">AudioMixer</a>
    /// When you create a <a href="https://docs.unity3d.com/ScriptReference/AudioSource.html">Audio source</a>,
    /// configure the output with this values
    /// </summary>
    public enum ENUM_AudioMixer
    {
        /// <summary>
        /// Master channel, it controls every single volume
        /// </summary>
        Master,

        /// <summary>
        /// The music channel, it is the background music
        /// </summary>
        Music,

        /// <summary>
        /// Sound effects
        /// </summary>
        SFx,

        /// <summary>
        /// The ui clicks and beeps
        /// </summary>
        UI,

        /// <summary>
        /// The ambience sounds, it is used with the music to provide a good experience
        /// </summary>
        Ambience,

        /// <summary>
        /// Voices in case your game needs it
        /// </summary>
        Voice
    }

    /// <summary>
    /// The data types supported by the <a href="#VrGamesDev.VRG_Session">VRG_Session</a> class
    /// </summary>
    public enum ENUM_DataType
    {
        /// <summary>
        /// A <a href="https://docs.microsoft.com/en-us/dotnet/api/system.boolean.parse?view=net-5.0">Bool data type</a> used in <a href="#VrGamesDev.VRG_RemoteBool">VRG_RemoteBool</a> struct by the <a href="#VrGamesDev.VRG_Remote">VRG_Remote</a> class
        /// </summary>
        BOOL,

        /// <summary>
        /// A <a href="https://docs.microsoft.com/en-us/dotnet/api/system.int32.parse?view=net-5.0">Int data type</a> used in <a href="#VrGamesDev.VRG_RemoteInt">VRG_RemoteInt</a> struct by the <a href="#VrGamesDev.VRG_Remote">VRG_Remote</a> class
        /// </summary>
        INT,

        /// <summary>
        /// A <a href="https://docs.microsoft.com/en-us/dotnet/api/system.single.parse?view=net-5.0">Float data type</a> used in <a href="#VrGamesDev.VRG_RemoteFloat">VRG_RemoteFloat</a> struct by the <a href="#VrGamesDev.VRG_Remote">VRG_Remote</a> class
        /// </summary>
        FLOAT,

        /// <summary>
        /// A <a href="https://docs.unity3d.com/ScriptReference/String.html">String data type</a> used in <a href="#VrGamesDev.VRG_RemoteString">VRG_RemoteString</a> struct by the <a href="#VrGamesDev.VRG_Remote">VRG_Remote</a> class
        /// </summary>
        STRING,
    }

    /// <summary>
    /// It just show the internal FSM status of the <a href="#VrGamesDev.VRG_Fader">VRG_Fader</a>
    /// </summary>
    public enum ENUM_Fader
    {
        /// <summary>
        /// The fader is not doing anything, all is visible
        /// </summary>
        NONE,

        /// <summary>
        /// The fader is fading in
        /// </summary>
        FADE_IN,

        /// <summary>
        /// The fader is fully faded, it can just fade out
        /// </summary>
        FADED,

        /// <summary>
        /// Th fader is fading out
        /// </summary>
        FADE_OUT,
    }

    /// <summary>
    /// The states of the FSM Mission's system of <a href="#VrGamesDev.VRG_Campaign">VRG_Campaign</a>
    /// </summary>
    public enum ENUM_Mission
    {
        /// <summary>
        /// When it fails the mission
        /// </summary>
        FAIL,

        /// <summary>
        /// It goes to the next one, from the current mission
        /// </summary>
        NEXT,

        /// <summary>
        /// THe mission was successfully completed
        /// </summary>
        PASS,

        /// <summary>
        /// IT was complted with excelence, it deserver a star
        /// </summary>
        STAR
    }

    /// <summary>
    /// Shows the internal FSM status of the <a href="#VrGamesDev.VRG_Remote">VRG_Remote</a>
    /// </summary>
    public enum ENUM_VRG_Remote
    {
        /// <summary>
        /// It is disable, or has not started
        /// </summary>
        NONE,

        /// <summary>
        /// It just inited, and starting its processes
        /// </summary>
        INIT,

        /// <summary>
        /// Waiting for the remote server config answer the petitions
        /// </summary>
        WAITING,

        /// <summary>
        /// Updating the local data with the remote data
        /// </summary>
        UPDATING,

        /// <summary>
        /// I'm ready, query me for info
        /// </summary>
        READY,
    }

    /// <summary>
    /// The kind of color to colorize an image or a renderer when a skin is set
    /// to any <a href="#VrGamesDev.VRG_UI">VRG_UI</a> component
    /// </summary>
    public enum ENUM_VRG_UIColor
    {
        /// <summary>
        /// This element will not be colored 
        /// </summary>
        NONE,

        /// <summary>
        /// Assign the background color and all its aspect
        /// </summary>
        BACKGROUND,

        /// <summary>
        /// Assign the foreground color
        /// </summary>
        FOREGROUND,

        /// <summary>
        /// Assign the third color from the VRG_Skin element
        /// </summary>
        THIRD,
    }

    /// <summary>
    /// The type of exception to make on this game object when evaluated by the <a href="#VrGamesDev.VRG_UI">VRG_UI</a>
    /// </summary>
    public enum ENUM_VRG_UIExcept
    {
        /// <summary>
        /// It will not be excepted
        /// </summary>
        NONE,

        /// <summary>
        /// Just except this game object
        /// </summary>
        SELF,

        /// <summary>
        /// It except itself and all its child, EVERYBODY
        /// </summary>
        ALL_CHILDREN,
    }

    /// <summary>
    /// Level of verbose, there are 7 level of verbose from silence up to full debug
    /// </summary>
    public enum ENUM_Verbose
    {
        /// <summary>
        /// ENUM_Verbose.NONE = Silence, NONE is always sent to the editor log window
        /// </summary>
        NONE = 0,

        /// <summary>
        /// ENUM_Verbose.<font color = red> ERROR </font> = When you need to show an error to the user and record a posible failure
        /// </summary>
        ERROR = 1,

        /// <summary>
        /// ENUM_Verbose.<font color = yellow> WARNING </font> = Something was cheesy, if something is wacky, strange, or unexpected
        /// </summary>
        WARNING = 2,

        /// <summary>
        /// ENUM_Verbose.<font color = green> LOGS </font> = To track some basic information, or to send logs to BHEL
        /// </summary>
        LOGS = 3,

        /// <summary>
        /// ENUM_Verbose.<font color = blue> DEBUG </font> = More information, ins and outs, flow of the game, useful for debug using asynchronous activities
        /// </summary>
        DEBUG = 4,

        /// <summary>
        /// ENUM_Verbose.<font color = cyan> ALL </font> = The most verbose, it basically shows EVERYTHING!!!
        /// </summary>
        ALL = 5
    }




    /// <summary>
	/// Static class, do not instantiate this class
	///
	/// This class has all the static methods, not related to any class
	/// 
    /// </summary>
    public class VRG : MonoBehaviour
    {
        /// <summary>
		/// Constant value: The frames the IsReady() functions will wait
		/// </summary>
        public static int IsReady = 30;

        /// <summary>
		/// Get the hexadecimal value of a Color. Example: Black = #000000
		/// </summary>
		/// <param name="valueLocal">a Color variable, </param>
		/// <returns>Get the Hexadecimal value of the color valueLocal</returns>
        public static string GetHtmlFromColor(Color valueLocal)
        {
            int r = (int)Mathf.RoundToInt(255 * valueLocal.r);
            int g = (int)Mathf.RoundToInt(255 * valueLocal.g);
            int b = (int)Mathf.RoundToInt(255 * valueLocal.b);

            return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        /// <summary>
		/// Get a Color32 variable with a random red, green, blue values.
		/// </summary>
		/// <returns>The randomized color</returns>
        public static Color32 GetRandomColor()
        {
            return new Color32
            (
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                255
            );
        }

        /// <summary>
		/// Receive a GameObject, to analize its route in the scene
		/// </summary>
		/// <param name="valueLocal"></param>
		/// <returns>The route to find the hierarchy in the Scene</returns>
        public static string GetSceneGameObject(GameObject valueLocal)
        {
            // Variable para guardar la ruta del padre
            string sMyParent = string.Empty;

            // in case this is called in the OnDestroy();
            if (valueLocal)
            {
                // Si es un gameobject y tiene transform
                if (valueLocal.transform)
                {
                    // guardando el padre de este objeto
                    Transform MyFather = valueLocal.transform.parent;

                    // mientras tenga padres, seguirlo buscando
                    while (MyFather)
                    {
                        // Se guarda la variable recursiva para todos los padres
                        sMyParent = MyFather.name + " -> " + sMyParent;

                        // reasigno el papa del papa para seguir evaluando
                        MyFather = MyFather.transform.parent;
                    }

                    // add the current object to end the route
                    sMyParent += valueLocal.transform.name;
                }
            }

            return sMyParent;
        }

        /// <summary>
        /// Constant value: The color order to colorize the verbose messages
        /// </summary>
        /// <param name="valueLocal">ENUM_Verbose: The verbose enumerator </param>
        /// <returns>The color in plain text</returns>
        // this method finds the color according to the level of verbosing
        public static string GetEnumColor(ENUM_Verbose valueLocal)
        {
            // color 
            string[] asVerboseColors =
            {
                "black",
                "red",
                "yellow",
                "green",
                "blue",
                "cyan"
            };

            // just find it in the current array, if not found ,return the first one.
            if (asVerboseColors[(int)(valueLocal)] != null)
            {
                return asVerboseColors[(int)(valueLocal)];
            }
            else
            {
                return asVerboseColors[0];
            }
        }

        /// #IGNORE
        public static string LongToBytes(long valueLocal) => LongToBytes(valueLocal, true);
        /// <summary>
        /// Transform the number bytes into a a string megabytes
        /// </summary>
        /// <param name="valueLocal">The value to transform (long)</param>
        /// <param name="addUnitLocal">By default true; you can get the value with Mb string</param>
        /// <returns></returns>
        public static string LongToBytes(long valueLocal, bool addUnitLocal)
        {
            string sReturn = string.Empty;

            sReturn +=
                (
                    ((float)valueLocal / 1000000).ToString("F2")
                );

            if (addUnitLocal)
            {
                sReturn += " Mb";
            }

            return sReturn.Trim();
        }

        /// <summary>
		/// Application.OpenURL, trimmed and escaped
		/// </summary>
		/// <param name="valueLocal">The url to open</param>
		/// <returns>if it succeed</returns>
        public static bool OpenUrl(string valueLocal)
        {
            bool bReturn = false;

            if (valueLocal.Trim() != string.Empty)
            {
                bReturn = true;

                Application.OpenURL(System.Uri.EscapeUriString(valueLocal));
            }

            return bReturn;
        }


        // funcion general para regresar un texto en el formato del camello
        public static string Camello(string valueLocal)
        {
            // si el texto a evaluar es nulo o esta vacio no hacer nada
            if (string.IsNullOrEmpty(valueLocal))
            {
                // se regresa string vacio
                return string.Empty;
            }

            // se regresa la primera mayuscula y el resto todo en minusculas
            return char.ToUpper(valueLocal[0]) + valueLocal.Substring(1).ToLower();
        }
    }
}
