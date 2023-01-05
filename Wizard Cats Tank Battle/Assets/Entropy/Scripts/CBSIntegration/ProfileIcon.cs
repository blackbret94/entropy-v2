using CBS;
using CBS.Scriptable;
using CBS.UI;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.Scripts.CBSIntegration
{
    public class ProfileIcon : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI NickNameLabel;
        [SerializeField]
        private TextMeshProUGUI CoinsLabel;
        [SerializeField]
        private AvatarDrawer Avatar;

        private IProfile CBSProfile { get; set; }
        private ICurrency CBSCurrency { get; set; }
        private ProfilePrefabs Prefabs { get; set; }
        
        private const string CURRENCY_CODE = "CC";

        private void Start()
        {
            Prefabs = CBSScriptable.Get<ProfilePrefabs>();
            CBSProfile = CBSModule.Get<CBSProfile>();
            CBSCurrency = CBSModule.Get<CBSCurrency>();
            // subscribe to events
            CBSProfile.OnDisplayNameUpdated += OnDisplayNameUpdated;
            CBSProfile.OnAvatarImageUpdated += OnAvatarImageUpdated;
            // try display cache value
            DisplayName();
            // get actual data from DB
            CBSProfile.GetAccountInfo(OnAccountInfoGetted);
            
            CBSCurrency.OnCurrencyUpdated += OnCurrencyUpdated;

            DrawAvatar();
            RefreshCurrency();
        }

        private void OnDestroy()
        {
            CBSProfile.OnDisplayNameUpdated -= OnDisplayNameUpdated;
            CBSProfile.OnAvatarImageUpdated -= OnAvatarImageUpdated;
        }

        private void DisplayName()
        {
            NickNameLabel.text = CBSProfile.DisplayName;
        }

        private void DrawAvatar()
        {
            // draw avatar
            var avatarUrl = CBSProfile.AvatarUrl;
            var profileID = CBSProfile.PlayerID;
            Avatar.LoadAvatarFromUrl(avatarUrl, profileID);
        }

        // button click
        public void ShowAccountInfo()
        {
            var windowsPrefab = Prefabs.AccountForm;
            UIView.ShowWindow(windowsPrefab);
        }

        private void OnDisplayNameUpdated(CBSUpdateDisplayNameResult result)
        {
            DisplayName();
        }

        private void OnAccountInfoGetted(CBSGetAccountInfoResult result)
        {
            DisplayName();
        }

        private void OnAvatarImageUpdated(CBSUpdateAvatarUrlResult obj)
        {
            DrawAvatar();
        }
        
        // Currency
        private void OnCurrencyUpdated(CBSUpdateCurrencyResult result)
        {
            if (result.IsSuccess)
            {
                Debug.LogFormat("Currency with code {0} was updated", result.CurrencyCode);
                if(result.CurrencyCode == CURRENCY_CODE)
                    CoinsLabel.text = result.CurrentValue.ToString();
            }
        }

        public void RefreshCurrency()
        {
            CBSCurrency.GetCurrencies(OnGetCurrencies);
        }
        
        private void OnGetCurrencies(CBSGetCurrenciesResult result)
        {
            if (result.IsSuccess)
            {
                if (result.Currencies.ContainsKey(CURRENCY_CODE))
                    CoinsLabel.text = result.Currencies[CURRENCY_CODE].ToString();
            }
            else
            {
                Debug.LogError("Error refreshing currency! " + result.Error);
            }
        }
    }
}