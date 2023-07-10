using System;
using CBS;
using UnityEngine;

namespace Entropy.Scripts.Currency
{
    // Turned this into a monobehavior singleton
    public class CurrencyTransaction : MonoBehaviour
    {
        private IAuth AuthModule;
        private ICurrency CBSCurrency { get; set; }
        private const string CURRENCY_CODE = "CC";
        private int _lastCurrency = 0;

        private static CurrencyTransaction _instance;
        public static CurrencyTransaction Instance => _instance;

        public event EventHandler<int> LocalCurrencyUpdated;
        public int LastCurrency => _lastCurrency;

        private void Start()
        {
            if(_instance != null)
                Destroy(gameObject);

            _instance = this;

            DontDestroyOnLoad(transform.gameObject);
            
            AuthModule = CBSModule.Get<CBSAuth>();

            AuthModule.OnLoginEvent += OnUserLogIn;
            AuthModule.OnLogoutEvent += OnUserLogout;
            
            CBSCurrency = CBSModule.Get<CBSCurrency>();
            CBSCurrency.OnCurrencyUpdated += OnCurrencyUpdated;
        }
        
        private void OnUserLogIn(CBSLoginResult result)
        {
            if (result.IsSuccess)
            {
                RefreshCurrency();
            }
        }
        
        private void OnUserLogout(CBSLogoutResult result)
        {
            if (result.IsSuccess)
            {
                _lastCurrency = 0;
            }
        }
        
        public CurrencyTransaction(bool refreshCurrency = true)
        {
            CBSCurrency = CBSModule.Get<CBSCurrency>();
            CBSCurrency.OnCurrencyUpdated += OnCurrencyUpdated;
            
            if(refreshCurrency)
                RefreshCurrency();
        }

        public void AddCurrency(int value)
        {
            CBSCurrency.AddUserCurrency(value, CURRENCY_CODE, OnUpdateCurrency);
            _lastCurrency += value;
            RaiseLocalCurrencyUpdated();
        }

        public void DecreaseCurrency(int value)
        {
            CBSCurrency.DecreaseUserCurrency(value, CURRENCY_CODE, OnUpdateCurrency);
            _lastCurrency -= value;
            RaiseLocalCurrencyUpdated();
        }

        public bool QueryPurchase(int cost)
        {
            return cost <= _lastCurrency;
        }
        
        private void OnUpdateCurrency(CBSUpdateCurrencyResult result)
        {
            if (result.IsSuccess)
            {
            }
            else
            {
                Debug.LogError("Error updating currency " + result.Error.Message);
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
                {
                    _lastCurrency = result.Currencies[CURRENCY_CODE];
                    RaiseLocalCurrencyUpdated();
                }
            }
            else
            {
                Debug.LogError("Error refreshing currency! " + result.Error.Message);
            }
        }

        private void RaiseLocalCurrencyUpdated()
        {
            LocalCurrencyUpdated?.Invoke(this, _lastCurrency);
        }
        
        private void OnCurrencyUpdated(CBSUpdateCurrencyResult result)
        {
            if (result.IsSuccess)
            {
                // Debug.LogFormat("Currency with code {0} was updated", result.CurrencyCode);
                if (result.CurrencyCode == CURRENCY_CODE)
                {
                    _lastCurrency = result.CurrentValue;
                    RaiseLocalCurrencyUpdated();
                }
                else
                {
                    Debug.LogError("Retrieved a currency with non-matching currency code");
                }
            }
            else
            {
                Debug.LogError("Error updating currency: " + result.Error.Message);
            }
        }
    }
}