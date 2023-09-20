using Entropy.Scripts.Currency;

namespace Entropy.Scripts.Player
{
    public class PlayerCurrencyRewarder
    {
        public const int KILL_REWARD = 25;
        private const int MATCH_BONUS = 75;
        private const int FIRST_PLACE_BONUS = 100;
        private const int SECOND_PLACE_BONUS = 50;
        private const int THIRD_PLACE_BONUS = 25;

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

        public int RewardForSecondPlace()
        {
            Reward(SECOND_PLACE_BONUS);
            return SECOND_PLACE_BONUS;
        }

        public int RewardForThirdPlace()
        {
            Reward(THIRD_PLACE_BONUS);
            return THIRD_PLACE_BONUS;
        }
        
        private void Reward(int value)
        {
            CurrencyTransaction.Instance.AddCurrency(value);
        }
    }
}