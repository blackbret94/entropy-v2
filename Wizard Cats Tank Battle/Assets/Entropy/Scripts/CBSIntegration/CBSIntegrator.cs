using UnityEngine;

namespace Vashta.Entropy.Scripts.CBSIntegration
{
    [RequireComponent(typeof(AuthState))]
    [RequireComponent(typeof(ProfileState))]
    public class CBSIntegrator : MonoBehaviour
    {
        public static CBSIntegrator Instance { get; private set; }
        public AuthState AuthState { get; private set; }
        public ProfileState ProfileState { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                AuthState = GetComponent<AuthState>();
                ProfileState = GetComponent<ProfileState>();
            }
        }
    }
}