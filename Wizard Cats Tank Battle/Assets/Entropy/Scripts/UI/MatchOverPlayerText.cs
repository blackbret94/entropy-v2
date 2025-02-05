using TanksMP;
using TMPro;
using UnityEngine;
using Vashta.Entropy.TanksExtensions;
using Vashta.Entropy.Character;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.UI
{
    public class MatchOverPlayerText : GamePanel
    {
        public TextMeshProUGUI PlayerName;
        public TextMeshProUGUI Tagline;
        [Tooltip("Is the player in 1st, 2nd, or 3rd?")]
        public int Place;

        public void Set(ScoreboardRowData playerScoreData, CharacterAppearance appearance)
        {
            string playerName = playerScoreData.Name;
            int points = playerScoreData.Kills;
            Color color = playerScoreData.Material.color;

            gameObject.SetActive(true);
            
            PlayerName.text = $"#{Place} {playerName}";
            PlayerName.color = color;
            
            Tagline.text = $"{points} {(points == 1 ? " Point" : " Points")}";

            PlayerController playerController = playerScoreData.PlayerController;
            
            if (playerController == null)
            {
                Debug.LogWarning("Warning: Player game object did not have Player component for Game Over screen!");
                Randomize(appearance, playerScoreData.TeamInstance);
                return;
            }
            
            Debug.Log("Copying outfit from player");
            appearance.CopyFromOtherPlayer(playerController.CharacterAppearance, playerScoreData.TeamInstance);
        }

        private void Randomize(CharacterAppearance appearance, TeamInstance teamInstance)
        {
            appearance.teamInstance = teamInstance;
            appearance.GetComponent<CharacterRandomAppearance>().Randomize(false);
            appearance.ColorizeCart();

        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}