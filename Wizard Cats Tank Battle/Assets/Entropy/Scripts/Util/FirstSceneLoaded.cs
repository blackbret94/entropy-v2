using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vashta.Entropy.Util
{
    public class FirstSceneLoaded : MonoBehaviour
    {
        private string _firstSceneLoaded;
        private static FirstSceneLoaded _instance;

        public static FirstSceneLoaded Get() => _instance;

        private void Awake()
        {
            if(_instance != null)
                Destroy(gameObject);
            
            _instance = this;
            _firstSceneLoaded = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(gameObject);
        }

        public bool IsFirstScene()
        {
            return _firstSceneLoaded == SceneManager.GetActiveScene().name;
        }
    }
}