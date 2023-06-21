using UnityEngine;

namespace Vashta.Entropy.UI.TeamScore
{
    public class TeamPlayerCounter : MonoBehaviour
    {
        public GameObject[] PlayerMarkers;

        private void Start()
        {
            foreach (var playerMarker in PlayerMarkers)
            {
                playerMarker.SetActive(false);
            }            
        }

        public void SetPlayerCount(int count)
        {
            for (int i = 0; i < PlayerMarkers.Length; i++)
            {
                PlayerMarkers[i].SetActive(i < count);
            }
        }
    }
}