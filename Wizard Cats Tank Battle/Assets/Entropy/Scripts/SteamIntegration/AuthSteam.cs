using System;
using CBS;
using CBS.Context;
using Steamworks;
using UnityEngine;

namespace Vashta.Entropy.Scripts.SteamIntegration
{
    public class AuthSteam : MonoBehaviour
    {
        private IAuth AuthModule;
        Callback<GetTicketForWebApiResponse_t> m_AuthTicketForWebApiResponseCallback;
        string m_SessionTicket;
        string identity = "unityauthenticationservice";

        void Start()
        {
            SignInWithSteam();
        }

        private void SignInWithSteam()
        {
            // It's not necessary to add event handlers if they are
            // already hooked up.
            // Callback.Create return value must be assigned to a
            // member variable to prevent the GC from cleaning it up.
            // Create the callback to receive events when the session ticket
            // is ready to use in the web API.
            // See GetAuthSessionTicket document for details.
            m_AuthTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);

            SteamUser.GetAuthTicketForWebApi(identity);
        }
        
        void OnAuthCallback(GetTicketForWebApiResponse_t callback)
        {
            m_SessionTicket = BitConverter.ToString(callback.m_rgubTicket).Replace("-", string.Empty);
            m_AuthTicketForWebApiResponseCallback.Dispose();
            m_AuthTicketForWebApiResponseCallback = null;
            Debug.Log("Steam Login success. Session Ticket: " + m_SessionTicket);
            // Call Unity Authentication SDK to sign in or link with Steam, displayed in the following examples, using the same identity string and the m_SessionTicket.
            
            AuthModule = CBSModule.Get<CBSAuth>();
            AuthModule.LoginWithSteam(m_SessionTicket, OnUserLogin);
            Debug.Log("Attempting steam login...");
        }

        private void OnUserLogin(CBSLoginResult result)
        {
            AuthContext authContext = FindFirstObjectByType<AuthContext>();

            if (result.IsSuccess)
            {
                Debug.Log(string.Format("User with ID {0} successfully log in", result.PlayerId));
                
                if (authContext != null)
                {
                    authContext.OnLoginComplete(result);
                }
            }
            else
            {
                Debug.LogError("Error logging into Steam! " + result.Error.Message);
                
                // Show login panel
                if (authContext != null)
                {
                    authContext.ShowLoginScreen();
                }
            }
        }

        public void UpdateUsername()
        {
            CBSModule.Get<CBSProfile>().GetAccountInfo(OnAccountInfoGetted);
        }
        
        private void OnAccountInfoGetted(CBSGetAccountInfoResult result)
        {
            if (result.IsSuccess)
            {
                if (SteamAPI.IsSteamRunning() && SteamUser.GetSteamID().IsValid())
                {
                    string playfabName = result.DisplayName;
                    string steamUsername = SteamFriends.GetPersonaName();

                    if (!string.IsNullOrEmpty(steamUsername) && playfabName != steamUsername)
                    {
                        CBSModule.Get<CBSProfile>().UpdateUserName(steamUsername);
                    }
                }
            }
        }
    }
}