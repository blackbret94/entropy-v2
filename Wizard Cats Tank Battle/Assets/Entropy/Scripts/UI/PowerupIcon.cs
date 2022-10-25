using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class PowerupIcon : GamePanel
    {
        public PowerupDirectory PowerupDirectory;
        public Image Icon;
        public GameObject[] AmmoIcons;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void SetPowerup(int bulletId, int ammo)
        {
            if (bulletId > 0)
            {
                gameObject.SetActive(true);
                Powerup powerup = PowerupDirectory[bulletId];
                Icon.sprite = powerup.Icon;
                Icon.color = powerup.Color;
            }

            if (bulletId == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                SetAmmo(ammo);
                
            }
        }

        public void SetAmmo(int ammoAmount)
        {
            for (int i = 0; i < AmmoIcons.Length; i++)
                AmmoIcons[i].SetActive(i < ammoAmount);
            
            if(ammoAmount == 0)
                gameObject.SetActive(false);
        }
    }
}