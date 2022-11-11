using System.Collections;
using TanksMP;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entropy.Scripts.Player
{
    public class PlayerBuffController: MonoBehaviour
    {
        public TanksMP.Player player;
        public float RefreshRateS;

        private float msLastRun;

        private float SpeedBoostModifier = 2f; // TODO: Move this into buff
        private float RapidFireModifier = .5f;
        private int SpikeDamageModifier = 15;
        
        private void Start()
        {
            msLastRun = Time.time;
            StartCoroutine(LoopCoroutine());
        }

        private IEnumerator LoopCoroutine()
        {
            while (true)
            {
                OneTimeStep();
                msLastRun = Time.time;
                yield return new WaitForSeconds(RefreshRateS + Random.Range(0f, .1f));
            }
        }

        // May eventually want to move this out of player properties if optimization is needed
        private void OneTimeStep()
        {
            int buffIndex = player.GetView().GetBuffIndex();

            if (buffIndex == 0)
                return;
            
            float startingBuffSeconds = player.GetView().GetBuffSeconds();
            
            if (startingBuffSeconds <= -.001)
                return;

            player.GetView().DecBuffSeconds(Time.time - msLastRun);
        }

        public float GetSpeedBonus()
        {
            if (CheckIfPowerUpIsActive(1)) // Remove hard coding later
                return SpeedBoostModifier;

            return 0;
        }

        public float GetRapidFireBonus()
        {
            if (CheckIfPowerUpIsActive(2)) // Remove hard coding later
                return RapidFireModifier;

            return 1;
        }

        public bool IsReflective()
        {
            return CheckIfPowerUpIsActive(3);
        }

        public int GetSpikeDamageModifier()
        {
            if (CheckIfPowerUpIsActive(4))
            {
                return 1 + SpikeDamageModifier;
            }
            
            return 0;
        }

        private bool CheckIfPowerUpIsActive(int powerUpId)
        {
            int buffIndex = player.GetView().GetBuffIndex();
            if (buffIndex != powerUpId)
                return false;
            
            float startingBuffSeconds = player.GetView().GetBuffSeconds();
            
            if (startingBuffSeconds <= -.001)
                return false;

            return true;
        }
    }
}