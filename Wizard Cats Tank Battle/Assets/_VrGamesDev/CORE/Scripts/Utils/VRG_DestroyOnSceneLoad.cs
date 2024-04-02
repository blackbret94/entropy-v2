using UnityEngine;
using UnityEngine.SceneManagement;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// This class allows a "DontDestroyOnLoad()" object, to be destroyed
    /// when a new scene is loaded
    /// </summary>
    public class VRG_DestroyOnSceneLoad : VRG_Base
    {
        // Use this for initialization
        // para decirle al engine que es una funcion nueva
        private new void OnEnable()
        {
            // I got noticed a new scene was loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            // FREEDOM!!!!
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // called second
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Object.Destroy(this.gameObject);
        }

    }
}