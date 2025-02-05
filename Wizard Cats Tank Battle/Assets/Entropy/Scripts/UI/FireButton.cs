using TanksMP;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vashta.Entropy.Player;

namespace Vashta.Entropy.UI
{
    public class FireButton : MonoBehaviour
    {
        [FormerlySerializedAs("Player")] public PlayerController playerController;
        public Slider Slider;

        public void Start()
        {
            Slider.value = 1;
        }

        private void Update()
        {
            if (playerController == null)
                return;

            Slider.value = playerController.CombatController.FractionFireReady;
        }
    }
}