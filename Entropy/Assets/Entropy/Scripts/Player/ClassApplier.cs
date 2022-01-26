namespace Entropy.Scripts.Player
{
    public class ClassApplier
    {
        public static void ApplyClass(TanksMP.Player player, PlayerCollisionHandler playerCollisionHandler, ClassDefinition classDefinition)
        {
            player.maxHealth = classDefinition.maxHealth;
            player.fireRate = classDefinition.fireRate;
            player.moveSpeed = classDefinition.moveSpeed;
            playerCollisionHandler.armor = classDefinition.armor;
            playerCollisionHandler.damageAmtOnCollision = classDefinition.damageAmtOnCollision;
        }
    }
}