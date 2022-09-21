using System;
using UnityEngine;

namespace Entropy.Scripts.Currency
{
    public class CurrencyTransaction
    {
        private string prefsKey = "currency";
        
        public int GetCurrency()
        {
            return PlayerPrefs.GetInt(prefsKey, 0);
        }

        public int ModifyCurrency(int delta)
        {
            int startingVal = GetCurrency();
            return SetCurrency(Math.Max(startingVal + delta, 0));
        }

        public int SetCurrency(int val)
        {
            PlayerPrefs.SetInt(prefsKey, val);
            Debug.Log("Player currency is now: " + val);
            return val;
        }

        public bool QueryPurchase(int cost)
        {
            int currencyVal = GetCurrency();
            return cost >= currencyVal;
        }
    }
}