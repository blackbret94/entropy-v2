using TanksMP;
using UnityEngine.UI;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI.MatchCreation
{
    public class MatchCreationPanel : GamePanel
    {
        public InputField NameInputField;
        public InputField PasswordInputField;
        public InputField MaxPlayersInputField;
        public MapSelectionSelector MapSelector;

        public CatNameGenerator CatNameGenerator;

        public void CreateMatch()
        {
            if (NameInputField.text == "")
                NameInputField.text = CatNameGenerator.GetRandomName();
            
            UIMain.GetInstance().CreateRoom(NameInputField.text);
        }
    }
}