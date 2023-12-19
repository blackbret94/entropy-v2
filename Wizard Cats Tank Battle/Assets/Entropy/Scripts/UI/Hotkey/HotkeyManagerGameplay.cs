using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI.Hotkey
{
    public class HotkeyManagerGameplay : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetButtonDown("Action1")) Action1();
            if(Input.GetButtonDown("CastUltimate")) CastUltimate();
        }

        private void Action1()
        {
            // drop spoon
            Player player = Player.GetLocalPlayer();

            if (player != null)
            {
                player.CommandDropCollectibles();
                UIGame.GetInstance().DropCollectiblesButton.gameObject.SetActive(false);
            }
        }

        private void CastUltimate()
        {
            Player player = Player.GetLocalPlayer();

            if (player != null)
            {
                player.TryCastUltimate();
            }
        }
    }
}