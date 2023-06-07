using CBS;
using UnityEngine;

namespace Entropy.Scripts.Currency
{
    public class CurrencyTransaction
    {
        private ICurrency CBSCurrency { get; set; }
        private const string CURRENCY_CODE = "CC";
        private int _lastCurrency = 0;

        public CurrencyTransaction(bool refreshCurrency = true)
        {
            CBSCurrency = CBSModule.Get<CBSCurrency>();
            CBSCurrency.OnCurrencyUpdated += OnCurrencyUpdated;
            
            if(refreshCurrency)
                RefreshCurrency();
        }

        public void AddCurrency(int delta)
        {
            CBSCurrency.AddUserCurrency(delta, CURRENCY_CODE, OnUpdateCurrency);
        }

        public void DecreaseCurrency(int value)
        {
            CBSCurrency.DecreaseUserCurrency(value, CURRENCY_CODE, OnUpdateCurrency);
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
                Debug.LogError("Error updating currency " + result.Error);
            }
        }
        
        private void RefreshCurrency()
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
                }
            }
            else
            {
                Debug.LogError("Error refreshing currency! " + result.Error);
            }
        }
        
        private void OnCurrencyUpdated(CBSUpdateCurrencyResult result)
        {
            if (result.IsSuccess)
            {
                Debug.LogFormat("Currency with code {0} was updated", result.CurrencyCode);
                if (result.CurrencyCode == CURRENCY_CODE)
                {
                    _lastCurrency = result.CurrentValue;
                }
            }
        }
    }
}