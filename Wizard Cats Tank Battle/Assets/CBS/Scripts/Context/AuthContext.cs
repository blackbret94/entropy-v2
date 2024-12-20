﻿using System;
using CBS.Scriptable;
using CBS.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace CBS.Context
{
    public class AuthContext : MonoBehaviour
    {
        [SerializeField]
        private string LobbyScene;

        private AuthPrefabs AuthUIData { get; set; }
        private LoginForm LoginForm { get; set; }
        private AuthData AuthData { get; set; }
        private IAuth Auth { get; set; }

        private AsyncOperationHandle<SceneInstance> _handle;

        private void Start()
        {
            AuthUIData = CBSScriptable.Get<AuthPrefabs>();
            AuthData = CBSScriptable.Get<AuthData>();
            Auth = CBSModule.Get<CBSAuth>();
            Init();
        }

        private void Init()
        {
            // show background
            // var backgroundPrefab = AuthUIData.Background;
            // UIView.ShowWindow(backgroundPrefab);
            // check auto login
            var autoLogin = PlayerPrefs.GetInt("autologin", 0) == 1;
            if (autoLogin)
            {
                var popupViewer = new PopupViewer();
                popupViewer.ShowLoadingPopup();
            
                Auth.AutoLogin(onAuth => { 
                    if (onAuth.IsSuccess)
                    {
                        OnLoginComplete(onAuth);
                    }
                    else
                    {
                        ShowLoginScreen();
                    }
                    popupViewer.HideLoadingPopup();
                });
            }
            else
            {
                ShowLoginScreen();
            }
        }

        private void ShowLoginScreen()
        {
            // show login screen
            var loginPrefab = AuthUIData.LoginForm;
            var loginWindow = UIView.ShowWindow(loginPrefab);
            LoginForm = loginWindow.GetComponent<LoginForm>();
            // subscribe to success login
            LoginForm.OnLogined += OnLoginComplete;
        }

        private void OnLoginComplete(CBSLoginResult result)
        {
            if (result.IsSuccess)
            {
                if (LoginForm != null)
                {
                    LoginForm.OnLogined -= OnLoginComplete;
                }
                _handle = Addressables.LoadSceneAsync(LobbyScene);
            }
        }

        private void OnDestroy()
        {
            Addressables.Release(_handle);
        }
    }
}
