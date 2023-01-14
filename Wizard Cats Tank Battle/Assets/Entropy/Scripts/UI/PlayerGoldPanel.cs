using CBS;
using Entropy.Scripts.Currency;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class PlayerGoldPanel: GamePanel
    {
        private ICurrency CBSCurrency { get; set; }
        private const string CURRENCY_CODE = "CC";
        
        public TextMeshProUGUI Text;
        

        private void Awake()
        {
            CBSCurrency = CBSModule.Get<CBSCurrency>();
            CBSCurrency.OnCurrencyUpdated += OnCurrencyUpdated;
            
            Refresh();
        }

        public override void Refresh()
        {
            base.Refresh();
            RefreshCurrency();
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
                    Text.text = result.Currencies[CURRENCY_CODE].ToString();
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
                    Text.text = result.CurrentValue.ToString();
                }
            }
        }
    }
}