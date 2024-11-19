using System.Collections;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class InitMinimap : MonoBehaviour
    {
        private TanksMP.Player _localPlayer;
        public bl_MiniMap MiniMap;
        
        private void Start()
        {
            StartCoroutine(AttachToPlayerCoroutine());
        }

        private IEnumerator AttachToPlayerCoroutine()
        {
            while (_localPlayer == null)
            {
                _localPlayer = TanksMP.Player.GetLocalPlayer();
                yield return null;
            }

            MiniMap.Target = _localPlayer.transform;
            Debug.Log("Player attached!");
        }
    }
}