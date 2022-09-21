using Entropy.Scripts.Currency;
using TMPro;

namespace Vashta.Entropy.UI
{
    public class PlayerGoldPanel: GamePanel
    {
        public TextMeshProUGUI Text;
        
        private CurrencyTransaction _currencyTransaction;

        private void Awake()
        {
            _currencyTransaction = new CurrencyTransaction();
            Refresh();
        }

        public int GetCurrency()
        {
            return _currencyTransaction.GetCurrency();
        }

        public override void Refresh()
        {
            base.Refresh();
            Text.text = GetCurrency().ToString();
        }
    }
}