using UnityEngine;
using Vashta.Entropy.Player;

namespace Entropy.Scripts.Player
{
    public class ClassController : MonoBehaviour
    {
        public void ApplyClass(PlayerController playerController, PlayerCollisionHandler playerCollisionHandler, ClassDefinition classDefinition, float modifier = 1f)
        {
            playerController.maxHealth = (int)(classDefinition.maxHealth * modifier);
            playerController.fireRate = classDefinition.fireRate*(1/modifier);
            playerController.moveSpeed = classDefinition.moveSpeed * modifier;
            playerController.PlayerViewController.SetClassIcon(classDefinition.classIcon);
            playerCollisionHandler.armor = classDefinition.armor;
            playerCollisionHandler.damageAmtOnCollision = classDefinition.damageAmtOnCollision;
        }
    }
}