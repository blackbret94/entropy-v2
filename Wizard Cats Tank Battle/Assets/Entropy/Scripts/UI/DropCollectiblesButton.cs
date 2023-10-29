using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class DropCollectiblesButton : MonoBehaviour
    {
        public void DropCollectiblesLocalPlayer()
        {
            Player localPlayer = Player.GetLocalPlayer();

            if (!localPlayer)
            {
                Debug.LogError("Could not find local player!");
                return;
            }
            
            localPlayer.CommandDropCollectibles();
            gameObject.SetActive(false);
        }
    }
}