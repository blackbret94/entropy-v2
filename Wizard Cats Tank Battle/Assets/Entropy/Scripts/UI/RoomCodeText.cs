using System.Collections;
using Fusion;
using TanksMP;
using TMPro;
using UnityEngine;
using Vashta.Entropy.PhotonExtensions;

namespace Vashta.Entropy.UI
{
    public class RoomCodeText : MonoBehaviour
    {
        public TextMeshProUGUI Text;

        public void Start()
        {
            StartCoroutine(DisplayTextCR());
        }

        private IEnumerator DisplayTextCR()
        {
            GameManager gameManager = GameManager.GetInstance();
            
            while (!gameManager.HasSpawned)
                yield return null;

            NetworkRunner runner = gameManager.Runner;
            SessionInfo sessionInfo = runner.SessionInfo;
            Text.text = "Room Code: " + sessionInfo.Name;
        }
    }
}