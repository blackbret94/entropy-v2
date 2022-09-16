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
                Randomize();
        }

        public void Randomize()
        {
            RandomizeHat();
            RandomizeBody();
            RandomizeCart();
            CharacterAppearance.ApplyOutfit();
            
            if(PanelToRefresh)
                PanelToRefresh.Refresh();
        }

        private void RandomizeHat()
        {
            CharacterAppearance.Hat = PlayerCharacterWardrobe.GetRandomHat();
        }

        private void RandomizeBody()
        {
            BodyType bodyType = PlayerCharacterWardrobe.GetRandomBodyType();
            Skin skin = bodyType.GetRandomSkin();

            CharacterAppearance.Body = bodyType;
            CharacterAppearance.Skin = skin;
        }

        private void RandomizeCart()
        {
            Cart cart = PlayerCharacterWardrobe.GetRandomCart();
            CharacterAppearance.Cart = cart;
        }

        private void RandomizeTurret()
        {
            
        }
    }
}