using UnityEngine;
using CBS;
using Vashta.Entropy.SceneNavigation;

namespace Vashta.Entropy.Scripts.CBSIntegration
{
    public class AuthState : MonoBehaviour
    {
        private IAuth AuthModule;
        
        public CBSLoginResult? LoggedInUser = null;
        public SceneNavigator SceneNavigator;

        private void Start()
        {
            AuthModule = CBSModule.Get<CBSAuth>();

            AuthModule.OnLoginEvent += OnUserLogIn;
            AuthModule.OnLogoutEvent += OnUserLogout;
        }
        
        private void OnUserLogIn(CBSLoginResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log(string.Format("User with ID {0} successfully logged in", result.PlayerId));
                LoggedInUser = result;
                CBSIntegrator.Instance.ProfileState.GetActiveUser();
            }
        }
        
        private void OnUserLogout(CBSLogoutResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log(string.Format("User with ID {0} successfully log out", result.PlayerId));
                LoggedInUser = null;
                SceneNavigator.GoToLogin();
            }
        }
    }
}