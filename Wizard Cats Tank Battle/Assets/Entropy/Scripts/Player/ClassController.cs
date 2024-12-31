using UnityEngine;

namespace Entropy.Scripts.Player
{
    public class ClassController : MonoBehaviour
    {
        public void ApplyClass(TanksMP.Player player, PlayerCollisionHandler playerCollisionHandler, ClassDefinition classDefinition, float modifier = 1f)
        {
            player.maxHealth = (int)(classDefinition.maxHealth * modifier);
            player.fireRate = classDefinition.fireRate*(1/modifier);
            player.moveSpeed = classDefinition.moveSpeed * modifier;
            player.PlayerViewController.SetClassIcon(classDefinition.classIcon);
            playerCollisionHandler.armor = classDefinition.armor;
            playerCollisionHandler.damageAmtOnCollision = classDefinition.damageAmtOnCollision;
        }
    }
}