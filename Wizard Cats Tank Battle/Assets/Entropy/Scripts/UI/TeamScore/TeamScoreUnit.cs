using System;
using System.Collections;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.TeamScore
{
    public class TeamScoreUnit : MonoBehaviour
    {
        public TextMeshProUGUI TeamScoreTextMP;
        public Text TeamScoreText;
        public Animator TextAnimator;
        [FormerlySerializedAs("TeamSizeSlider")] public Slider TeamScoreSlider;
        public int TeamIndex;
        public TeamPlayerCounter PlayerCounter;
        
        private void Start()
        {
            TeamScoreSlider.value = 0;
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitForSeconds(.1f);
            TeamScoreSlider.maxValue = GameManager.GetInstance().maxScore;
        }
        
        public void UpdateScore(int score)
        {
            if (TeamScoreText != null)
            {
                if (score == Int32.Parse(TeamScoreText.text))
                    return;
                
                TeamScoreText.text = score.ToString();
            }

            if (TeamScoreTextMP != null)
            {
                if (score == Int32.Parse(TeamScoreTextMP.text))
                    return;
                
                TeamScoreTextMP.text = score.ToString();
            }
            
            if (TextAnimator != null)
                TextAnimator.Play("Animation");

            TeamScoreSlider.maxValue = GameManager.GetInstance().maxScore;
            TeamScoreSlider.value = score;
        }

        public void UpdateNumberOfPlayers(int newSize)
        {
            if (PlayerCounter == null)
                return;
            
            PlayerCounter.SetPlayerCount(newSize);
        }
    }
}