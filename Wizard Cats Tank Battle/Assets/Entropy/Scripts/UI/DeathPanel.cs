using TanksMP;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class DeathPanel : GamePanel
    {
        public Text deathText;
        public Text spawnDelayText;
        public Text tipText;
        public ScriptableTextList tipList;

        public void Set(string playerName, Team team)
        {
            //show killer name and colorize the name converting its team color to an HTML RGB hex value for UI markup
            deathText.text = "KNOCKED OUT BY \n<color=#" + ColorUtility.ToHtmlStringRGB(team.material.color) + ">" + playerName + "</color>";

            tipText.text = "TIP: " + tipList.getRandomString();
        }

        public void Clear()
        {
            deathText.text = string.Empty;
            spawnDelayText.text = string.Empty;
            tipText.text = string.Empty;
        }
        
        /// <summary>
        /// Set respawn delay value displayed to the absolute time value received.
        /// The remaining time value is calculated in a coroutine by GameManager.
        /// </summary>
        public void SetSpawnDelay(float time)
        {                
            spawnDelayText.text = Mathf.Ceil(time) + "";
        }

    }
}