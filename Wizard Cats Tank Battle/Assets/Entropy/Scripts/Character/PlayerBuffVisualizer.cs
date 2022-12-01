using System;
using UnityEngine;

namespace Vashta.Entropy.Character
{
    public class PlayerBuffVisualizer : MonoBehaviour
    {
        public GameObject SpeedBoostBuffVisualizer;
        public GameObject RapidFireBuffVisualizer;
        public GameObject SpikeDamageBuffVisualizer;
        public GameObject ReflectionBuffVisualizer;

        private void Start()
        {
            ToggleAll(false);
        }

        private void ToggleAll(bool enable)
        {
            ToggleSpeedBoostVisualizer(enable);
            ToggleRapidFireVisualizer(enable);
            ToggleReflectionVisualizer(enable);
            ToggleSpikeDamageVisualizer(enable);
        }

        // This is hacky, replace later
        public void ToggleVisualizerById(int buffId, bool enable)
        {
            ToggleAll(false);
            
            switch (buffId)
            {
                case 1:
                    ToggleSpeedBoostVisualizer(enable);
                    break;
                
                case 2:
                    ToggleRapidFireVisualizer(enable);
                    break;
                
                case 3:
                    ToggleReflectionVisualizer(enable);
                    break;
                
                case 4:
                    ToggleSpikeDamageVisualizer(enable);
                    break;
            }
        }

        public void ToggleSpeedBoostVisualizer(bool enable)
        {
            ToggleVisualizer(SpeedBoostBuffVisualizer, enable);
        }
        
        public void ToggleRapidFireVisualizer(bool enable)
        {
            ToggleVisualizer(RapidFireBuffVisualizer, enable);
        }
        
        public void ToggleSpikeDamageVisualizer(bool enable)
        {
            ToggleVisualizer(SpikeDamageBuffVisualizer, enable);
        }
        
        public void ToggleReflectionVisualizer(bool enable)
        {
            ToggleVisualizer(ReflectionBuffVisualizer, enable);
        }

        private void ToggleVisualizer(GameObject go, bool enable)
        {
            go.SetActive(enable);
        }
    }
}