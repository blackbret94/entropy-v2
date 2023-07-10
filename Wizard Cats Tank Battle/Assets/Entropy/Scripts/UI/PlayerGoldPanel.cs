using System;
using CBS;
using Entropy.Scripts.Currency;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class PlayerGoldPanel: GamePanel
    {
        private CurrencyTransaction CurrencyTransaction { get; set; }
        private const string CURRENCY_CODE = "CC";
        
        public TextMeshProUGUI Text;
        public Animator Animator;
        public string AnimationTrigger = "CoinRewarded";
        
        private void Awake()
        {
            CurrencyTransaction = CurrencyTransaction.Instance;
            CurrencyTransaction.LocalCurrencyUpdated += OnCurrencyUpdated;

            Text.text = CurrencyTransaction.LastCurrency.ToString();
        }

        private void OnDestroy()
        {
            CurrencyTransaction.LocalCurrencyUpdated -= OnCurrencyUpdated;
        }

        private void OnCurrencyUpdated(object sender, int result)
        {
            if (Int32.Parse(Text.text) == result)
                return;
            
            Text.text = result.ToString();
            
            if(Animator != null)
                Animator.SetTrigger(AnimationTrigger);
        }
    }
}