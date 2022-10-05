using UnityEngine;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI;

namespace Vashta.Entropy.Character
{
    public class CharacterRandomAppearance: MonoBehaviour
    {
        public CharacterAppearance CharacterAppearance;
        public PlayerCharacterWardrobe PlayerCharacterWardrobe;
        public bool RandomizeOnLoad = true;
        public GamePanel PanelToRefresh;

        private void Start()
        {
            if (RandomizeOnLoad)
                Randomize(false);
        }

        public void Randomize(bool useInventory)
        {
            RandomizeHat(useInventory);
            RandomizeBody(useInventory);
            RandomizeCart(useInventory);
            RandomizeTurret(useInventory);
            RandomizeMeow(useInventory);
            CharacterAppearance.ApplyOutfit();
            
            if(PanelToRefresh)
                PanelToRefresh.Refresh();
        }

        private void RandomizeHat(bool useInventory)
        {
            if (useInventory)
                CharacterAppearance.Hat = CharacterAppearance.PlayerInventory.GetRandomHat();
            else
                CharacterAppearance.Hat = PlayerCharacterWardrobe.GetRandomHat();
        }

        private void RandomizeBody(bool useInventory)
        {
            BodyType bodyType;
            Skin skin;
            
            if (useInventory)
            {
                bodyType = CharacterAppearance.PlayerInventory.GetRandomBodyType();
                skin = bodyType.GetRandomSkin();
            }
            else
            {
                bodyType = PlayerCharacterWardrobe.GetRandomBodyType();
                skin = bodyType.GetRandomSkin();
            }
            
            CharacterAppearance.Body = bodyType;
            CharacterAppearance.Skin = skin;
        }

        private void RandomizeCart(bool useInventory)
        {
            if(useInventory)
                CharacterAppearance.Cart = CharacterAppearance.PlayerInventory.GetRandomCart();
            else
                CharacterAppearance.Cart = PlayerCharacterWardrobe.GetRandomCart();
        }

        private void RandomizeTurret(bool useInventory)
        {
            if(useInventory)
                CharacterAppearance.Turret = CharacterAppearance.PlayerInventory.GetRandomTurret();
            else
                CharacterAppearance.Turret = PlayerCharacterWardrobe.GetRandomTurret();
        }

        private void RandomizeMeow(bool useInventory)
        {
            if(useInventory)
                CharacterAppearance.Meow = CharacterAppearance.PlayerInventory.GetRandomMeow();
            else
                CharacterAppearance.Meow = PlayerCharacterWardrobe.GetRandomMeow();
        }
    }
}