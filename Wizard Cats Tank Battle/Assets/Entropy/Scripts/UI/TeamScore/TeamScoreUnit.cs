using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.TeamScore
{
    public class TeamScoreUnit : MonoBehaviour
    {
        public TextMeshProUGUI TeamScoreTextMP;
        public Text TeamScoreText;
        public Animator TextAnimator;
        public Slider TeamSizeSlider;
        public int TeamIndex;

        public void UpdateScore(int score)
        {
            if (TextAnimator != null)
                TextAnimator.Play("Animation");

            if(TeamScoreText != null)
                TeamScoreText.text = score.ToString();
            
            if(TeamScoreTextMP != null)
                TeamScoreTextMP.text = score.ToString();
        }

        public void UpdateNumberOfPlayers(int newSize)
        {
            TeamSizeSlider.value = newSize;
        }
    }
}