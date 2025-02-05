using Entropy.Scripts.Player;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.UI
{
    public class DropCollectiblesButton : MonoBehaviour
    {
        public void DropCollectiblesLocalPlayer()
        {
            PlayerController localPlayerController = PlayerList.GetLocalPlayer();

            if (!localPlayerController)
            {
                Debug.LogError("Could not find local player!");
                return;
            }
            
            localPlayerController.DropCollectibles();
            gameObject.SetActive(false);
        }
    }
}