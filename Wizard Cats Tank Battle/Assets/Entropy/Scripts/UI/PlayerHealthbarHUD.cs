using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class PlayerHealthbarHUD : GamePanel
    {
        public TeamDefinitionDictionary TeamDefinitionDictionary;
        public Image HealthbarSprite;
        public Image ArmorbarSprite;

        public void SetTeam(string TeamName)
        {
            TeamDefinition teamDefinition = TeamDefinitionDictionary[TeamName];

            if (!teamDefinition)
            {
                Debug.LogError("Could not find team with ID: " + TeamName);
                return;
            }

            HealthbarSprite.sprite = teamDefinition.HealthBarSprite;
        }

        public void SetTeam(TeamDefinition teamDefinition)
        {
            if (!teamDefinition)
            {
                Debug.LogError("Missing team definition!");
                return;
            }

            HealthbarSprite.sprite = teamDefinition.HealthBarSprite;
        }
    }
}