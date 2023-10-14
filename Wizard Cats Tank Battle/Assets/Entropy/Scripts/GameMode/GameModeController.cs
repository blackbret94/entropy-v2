using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Vashta.Entropy.PhotonExtensions;

namespace Vashta.Entropy.GameMode
{
    public class GameModeController : MonoBehaviour
    {
        public GameObject TdmRoot;
        public GameObject CtfRoot;

        private RoomOptionsReader _roomOptionsReader;


        private void Start()
        {
            _roomOptionsReader = new RoomOptionsReader();
            
            SetGameMode(_roomOptionsReader.GetGameMode());
        }

        public void SetGameMode(TanksMP.GameMode gameMode)
        {
            switch (gameMode)
            {
                case TanksMP.GameMode.TDM:
                    SetGameModeTDM();
                    break;
                
                case TanksMP.GameMode.CTF:
                    SetGameModeCTF();
                    break;
            }
        }
        
        private void SetGameModeTDM()
        {
            // Debug.Log("Setting game mode to TDM");

            if(CtfRoot != null)
                CtfRoot.SetActive(false);
            
            if(TdmRoot != null)
                TdmRoot.SetActive(true);
        }

        private void SetGameModeCTF()
        {
            // Debug.LogError("Setting game mode to CTF");
            
            if(TdmRoot != null)
                TdmRoot.SetActive(false);
            
            if(CtfRoot != null)
                CtfRoot.SetActive(true);
        }
    }
}