using Photon.Pun;
using TanksMP;
using TMPro;
using UnityEngine;
using Vashta.Entropy.TanksExtensions;
using Vashta.Entropy.Character;

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
            
            GameObject playerGo = NetworkManagerCustom.GetInstance().GetPlayerGameObject(playerScoreData.Player);

            // TODO: Handle copying bot outfits
            if (playerGo == null)
            {
                Debug.LogWarning("Warning: Could not find game object for player for Game Over screen!");
                Randomize(appearance, playerScoreData.Team);
                return;
            }

            Player player = playerGo.GetComponent<Player>();
            
            if (player == null)
            {
                Debug.LogWarning("Warning: Player game object did not have Player component for Game Over screen!");
                Randomize(appearance, playerScoreData.Team);
                return;
            }
            
            Debug.Log("Copying outfit from player");
            appearance.CopyFromOtherPlayer(player.CharacterAppearance, playerScoreData.Team);
        }

        private void Randomize(CharacterAppearance appearance, Team team)
        {
            appearance.Team = team;
            appearance.GetComponent<CharacterRandomAppearance>().Randomize(false);
            appearance.ColorizeCart();

        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}