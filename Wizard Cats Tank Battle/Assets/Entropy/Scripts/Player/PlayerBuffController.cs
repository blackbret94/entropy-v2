using System.Collections;
using TanksMP;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class PlayerBuffController: MonoBehaviour
    {
        public TanksMP.Player player;
        public float RefreshRateS;

        private float msLastRun;

        private const float SPEED_BOOST = 2f; // TODO: Move this into buff
        
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
            int buffIndex = player.GetView().GetBuffIndex();
            if (buffIndex != 1) // TODO: Remove the hardcoded index
                return 0;
            
            float startingBuffSeconds = player.GetView().GetBuffSeconds();
            
            if (startingBuffSeconds <= -.001)
                return 0;

            return SPEED_BOOST;
        }
    }
}