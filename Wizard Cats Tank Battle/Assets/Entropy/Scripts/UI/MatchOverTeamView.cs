using TanksMP;
using UnityEngine;
using Vashta.Entropy.Character;
using Vashta.Entropy.TanksExtensions;

namespace Vashta.Entropy.UI
{
    public class MatchOverTeamView : MonoBehaviour
    {
        public CharacterAppearance FirstPlaceRoot,
            SecondPlaceRoot,
            ThirdPlaceRoot;

        public MatchOverPlayerText
            FirstPlaceText,
            SecondPlaceText,
            ThirdPlaceText;

        public void Activate(ScoreboardRowData player1, ScoreboardRowData player2, ScoreboardRowData player3)
        {
            gameObject.SetActive(true);

            if (player1 != null)
                FirstPlaceText.Set(player1, FirstPlaceRoot);
            else
                FirstPlaceText.Hide();

            if (player2 != null)
                SecondPlaceText.Set(player2, SecondPlaceRoot);
            else
                SecondPlaceText.Hide();
            
            if(player3 != null)
                ThirdPlaceText.Set(player3, ThirdPlaceRoot);
            else 
                ThirdPlaceText.Hide();
        }
    }
}