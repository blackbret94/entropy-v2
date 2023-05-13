using Entropy.Scripts.Currency;

namespace Entropy.Scripts.Player
{
    public class PlayerCurrencyRewarder
    {
        private CurrencyTransaction _currencyTransaction;

        private const int KILL_REWARD = 25;
        private const int MATCH_BONUS = 50;
        private const int FIRST_PLACE_BONUS = 100;
        
        public PlayerCurrencyRewarder(bool refreshCurrency = true)
        {
            _currencyTransaction = new CurrencyTransaction(refreshCurrency);
        }

        public int RewardForKill()
        {
            Reward(KILL_REWARD);
            return KILL_REWARD;
        }

        public int RewardForMatchCompleted()
        {
            Reward(MATCH_BONUS);
            return MATCH_BONUS;
        }

        public int RewardForFirstPlace()
        {
            Reward(FIRST_PLACE_BONUS);
            return FIRST_PLACE_BONUS;
        }
        
        private void Reward(int value)
        {
            _currencyTransaction.AddCurrency(value);
        }
    }
}