using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.GameMode
{
    public class GameModeController : MonoBehaviour
    {
        public GameObject TdmRoot;
        public GameObject CtfRoot;
        public GameObject CtfsRoot;
        public GameObject KothRoot;
        public GameObject KothsRoot;

        private NetworkManagerCustom _networkManager;

        private void Awake()
        {
            _networkManager = NetworkManagerCustom.GetInstance();
            
            SetGameMode(_networkManager.LocalPlayerInfo.GameModeEnum);
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
                
                case TanksMP.GameMode.CTFS:
                    SetGameModeCTFS();
                    break;
                
                case TanksMP.GameMode.KOTH:
                    SetGameModeKOTH();
                    break;
                
                case TanksMP.GameMode.KOTHS:
                    SetGameModeKOTHS();
                    break;
            }
        }

        private void DeactivateAll()
        {
            if(CtfRoot != null)
                CtfRoot.SetActive(false);

            if(CtfsRoot != null)
                CtfsRoot.SetActive(false);
            
            if (TdmRoot != null)
                TdmRoot.SetActive(false);
            
            if(KothRoot != null)
                KothRoot.SetActive(false);
            
            if(KothsRoot != null)
                KothsRoot.SetActive(false);
        }
        
        private void SetGameModeTDM()
        {
            DeactivateAll();

            if (TdmRoot != null)
            {
                TdmRoot.SetActive(true);
            }
        }

        private void SetGameModeCTF()
        {
            DeactivateAll();

            if (CtfRoot != null)
            {
                CtfRoot.SetActive(true);
            }
        }

        private void SetGameModeCTFS()
        {
            DeactivateAll();

            if (CtfsRoot != null)
            {
                CtfsRoot.SetActive(true);
            }
        }

        private void SetGameModeKOTH()
        {
            DeactivateAll();

            if (KothRoot != null)
            {
                KothRoot.SetActive(true);
            }
        }

        private void SetGameModeKOTHS()
        {
            DeactivateAll();

            if (KothsRoot != null)
            {
                KothsRoot.SetActive(true);
            }
        }
    }
}