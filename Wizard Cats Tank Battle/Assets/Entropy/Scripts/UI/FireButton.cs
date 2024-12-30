using TanksMP;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class FireButton : MonoBehaviour
    {
        public Player Player;
        public Slider Slider;

        public void Start()
        {
            Slider.value = 1;
        }

        private void Update()
        {
            if (Player == null)
                return;

            Slider.value = Player.CombatController.FractionFireReady;
        }
    }
}