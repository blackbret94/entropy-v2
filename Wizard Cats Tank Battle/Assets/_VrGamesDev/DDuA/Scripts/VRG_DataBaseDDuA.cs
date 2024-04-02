using UnityEngine;

namespace VrGamesDev.DDuA
{
    ///#IGNORE
    public class AddressableSceneAttribute : PropertyAttribute { }

    /// <summary>
    /// The append mode, there are 3 modes, Overwrite, Append, diferent files
    /// </summary>
    public enum ENUM_AddressableStatus
    {
        /// <summary>
        /// It haven't started, it is empty and uninited
        /// </summary>
        NONE,

        /// <summary>
        /// It tried, but failed, since it couldn't continue, the error was logged to BHEL
        /// </summary>
        FAILED,

        /// <summary>
        /// The gameobject was destroyed, it could restart and create a new one
        /// </summary>
        DESTROYED,

        /// <summary>
        /// The path of the addressable is being queried
        /// </summary>
        PATHING,

        /// <summary>
        /// The address is not valid, the path was not found
        /// </summary>
        PATH_NOT_FOUND,

        /// <summary>
        /// There are more than one possible adddress, please check your group 
        /// </summary>
        MORE_THAN_ONE_PATH,

        /// <summary>
        /// The addressable is being queried to get its size
        /// </summary>
        SIZING,

        /// <summary>
        /// The addressable was sized, check the size property to get its value
        /// </summary>
        SIZED,

        /// <summary>
        /// The addressable is downloading, when it is done, the status wil be DOWNLOADED
        /// </summary>
        DOWNLOADING,

        /// <summary>
        /// The addressable was downloaded, check the prefab property to get its value
        /// </summary>
        DOWNLOADED,

        /// <summary>
        /// The addressable is loaded into memory
        /// </summary>
        LOADING_MEMORY,

        /// <summary>
        /// The addressable is ready, the prefab is loaded waiting to get instantiated
        /// </summary>
        PREFAB,

        /// <summary>
        /// The addressable was a scene and is now ready and loaded
        /// </summary>
        SCENE_LOADED,
    }


    /// <summary>
    /// This ENUM lets you define the behiavor SIZE, DOWNLOAD, READY, CREATE
    /// </summary>
    public enum ENUM_AddressableEvolution
    {
        /// <summary>
        /// Check for the size of the addressable, 0 if the addressable was already downloaded and is CACHED
        /// </summary>
        SIZE,

        /// <summary>
        /// Check for the size, and downloaded the addressable it not CACHED
        /// </summary>
        DOWNLOAD,

        /// <summary>
        /// Download the addressable and get it ready to be instantiated
        /// </summary>
        READY,

        /// <summary>
        /// Download, and instantiate a prefab.
        /// </summary>
        CREATE,
    }

}