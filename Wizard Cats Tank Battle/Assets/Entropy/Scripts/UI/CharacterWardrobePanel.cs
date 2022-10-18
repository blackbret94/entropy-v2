using Vashta.Entropy.Character;

namespace Vashta.Entropy.UI
{
    // Is this even used?
    public class CharacterWardrobePanel
    {
        public CharacterAppearance CharacterAppearance;
        
        public void SaveOutfit()
        {
            CharacterAppearance.SaveAppearance();
        }

        public void ReturnToMainMenu()
        {
            
        }

        public void SaveAndReturnToMainMenu()
        {
            SaveOutfit();
        }
    }
}