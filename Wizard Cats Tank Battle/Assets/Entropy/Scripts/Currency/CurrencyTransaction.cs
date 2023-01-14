using CBS;
using UnityEngine;

namespace Entropy.Scripts.Currency
{
    public class CurrencyTransaction
    {
        private ICurrency CBSCurrency { get; set; }
        private const string CURRENCY_CODE = "CC";
        private int _lastCurrency = 0;

        public CurrencyTransaction()
        {
            CBSCurrency = CBSModule.Get<CBSCurrency>();
            CBSCurrency.OnCurrencyUpdated += OnCurrencyUpdated;
            
            RefreshCurrency();
        }

        public void ModifyCurrency(int delta)
        {
            Debug.Log("Attempting to update currency");
            CBSCurrency.AddUserCurrency(delta, CURRENCY_CODE, OnUpdateCurrency);
        }

        public bool QueryPurchase(int cost)
        {
            return cost <= _lastCurrency;
        }
        
        private void OnUpdateCurrency(CBSUpdateCurrencyResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("The user has successfully received the currency");
            }
            else
            {
                Debug.LogError("Error updating currency " + result.Error);
            }
        }
        
        private void RefreshCurrency()
        {
            Debug.Log("Getting currency");
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