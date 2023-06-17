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

            ActivateOneCharacter(player1, FirstPlaceRoot, FirstPlaceText);
            ActivateOneCharacter(player2, SecondPlaceRoot, SecondPlaceText);
            ActivateOneCharacter(player3, ThirdPlaceRoot, ThirdPlaceText);
        }

        private void ActivateOneCharacter(ScoreboardRowData player, CharacterAppearance root, MatchOverPlayerText text)
        {
            if (player != null)
            {
                root.gameObject.SetActive(true);
                text.Set(player, root);
            }
            else
            {
                root.gameObject.SetActive(false);
                text.Hide();
            }
        }
    }
}