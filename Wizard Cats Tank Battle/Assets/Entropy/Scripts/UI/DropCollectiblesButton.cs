using Entropy.Scripts.Player;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class DropCollectiblesButton : MonoBehaviour
    {
        public void DropCollectiblesLocalPlayer()
        {
            Player localPlayer = PlayerList.GetLocalPlayer();

            if (!localPlayer)
            {
                Debug.LogError("Could not find local player!");
                return;
            }
            
            localPlayer.DropCollectibles();
            gameObject.SetActive(false);
        }
    }
}